using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AiCtf.Sdk
{
    /// <summary>
    /// Controls a game of capture the flag between teams controlled by opposing artificial intelligence
    /// </summary>
    public class CtfGame
    {
        /// <summary>
        /// The state of all turns in this game
        /// </summary>
        [JsonProperty]
        public IList<CtfGameState> States { get; private set; }

        /// <summary>
        /// The rules of this game
        /// </summary>
        [JsonProperty]
        public CtfGameRules Rules { get; private set; }

        /// <summary>
        /// Spawn positions for entities in this game
        /// </summary>
        [JsonProperty]
        public Dictionary<int, Vector2> SpawnPositions { get; private set; }

        /// <summary>
        /// The id of the team that won this game, if the game has ended
        /// </summary>
        [JsonProperty]
        public int? WinningTeam { get; private set; }

        /// <summary>
        /// The reason that the game ended
        /// </summary>
        [JsonProperty]
        public string GameEndReason { get; private set; }

        /// <summary>
        /// Is this game finished?
        /// </summary>
        [JsonIgnore]
        public bool IsGameOver
        {
            get
            {
                return !string.IsNullOrEmpty(GameEndReason) || WinningTeam != null;
            }
        }

        //Keep track of the last turn that each ship fired, for cooldown tracking
        private Dictionary<int, int> m_lastFireTurns = new Dictionary<int, int>();

        //All AIs participating in this game and their team ids
        private Dictionary<int, ICtfAi> m_ais = new Dictionary<int, ICtfAi>();

        //Current turn state
        private CtfGameState m_currentState;

        /// <summary>
        /// Constructor for deserialising
        /// </summary>
        [JsonConstructor]
        internal CtfGame()
        {
        }

        /// <summary>
        /// Create a ctf game
        /// </summary>
        /// <param name="rules">The rules of this game</param>
        /// <param name="ais">All AIs that will be competing in this game</param>
        public CtfGame(CtfGameRules rules, IList<ICtfAi> ais)
        {
            if (rules == null)
            {
                throw new ArgumentNullException("rules");
            }

            if (ais == null)
            {
                throw new ArgumentNullException("ais");
            }

            SpawnPositions = new Dictionary<int, Vector2>();
            Rules = rules;
            States = new List<CtfGameState>(Rules.TurnLimit);

            //Calculate spacing of objects for spawn positions
            float teamSpacing = MathHelper.TwoPi / ais.Count;
            float shipSpacing = MathHelper.TwoPi / Rules.ShipsPerTeam;

            float flagRadius = Rules.ArenaRadius - (2 * Rules.ShipRadius) - Rules.FlagRadius;

            CtfGameState initialState = new CtfGameState();

            initialState.Teams = new List<Team>(ais.Count);
            int teamIndex = 0;

            //Initialise all the teams
            foreach (ICtfAi ai in ais)
            {
                var team = new Team()
                {
                    Name = ai.Name ?? "Unnamed AI"
                };

                team.Flag = new Flag()
                {
                    Bounds = new BoundingCircle(Vector2.FromPolar(flagRadius, teamSpacing * teamIndex), Rules.FlagRadius),
                    Owner = team.Id,
                };
                SpawnPositions[team.Flag.Id] = team.Flag.Position;

                m_ais[team.Id] = ai;

                team.Ships = new List<Ship>();

                for (int i = 0; i < rules.ShipsPerTeam; i++)
                {
                    var ship = new Ship()
                    {
                        Bounds = new BoundingCircle(team.Flag.Position + Vector2.FromPolar(Rules.FlagRadius + Rules.ShipRadius, i * shipSpacing), Rules.ShipRadius),
                        Owner = team.Id,
                        Rotation = (i * shipSpacing),
                    };

                    SpawnPositions[ship.Id] = ship.Position;
                    m_lastFireTurns[ship.Id] = int.MinValue + 1;

                    team.Ships.Add(ship);
                }

                team.Projectiles = new List<Projectile>();

                initialState.Teams.Add(team);

                teamIndex++;
            }

            States.Add(initialState);
            m_currentState = initialState;
        }

        /// <summary>
        /// Initialise each AI team
        /// </summary>
        public void Initialise()
        {
            foreach (var ai in m_ais)
            {
                try
                {
                    ai.Value.Initialize(Rules.Clone(), ai.Key);
                }
                catch
                {
                    string failReason = string.Format("AI {0} ({1}) failed to initialise", ai.Value.Name, ai.Key);
                    GameEndReason = failReason;
                    m_currentState.Events.Add(failReason);
                    break;
                }
            }
        }

        /// <summary>
        /// Advance the game by one turn
        /// </summary>
        public void Step()
        {
            if (IsGameOver)
            {
                return;
            }

            //Check for win condition
            if (States.Count == Rules.TurnLimit || m_currentState.Teams.Any(t => t.FlagCaptures == Rules.FlagLimit))
            {
                var winningTeam = m_currentState.Teams.OrderByDescending(t => t.FlagCaptures).ThenByDescending(t => t.Kills).FirstOrDefault();

                if (States.Count == Rules.TurnLimit)
                {
                    GameEndReason = "Turn limit reached";
                }
                else if (m_currentState.Teams.Any(t => t.FlagCaptures == Rules.FlagLimit))
                {
                    GameEndReason = string.Format("AI {0} ({1}) reached the flag capture limit", winningTeam.Name, winningTeam.Id);
                }

                m_currentState.Events.Add(GameEndReason);

                WinningTeam = winningTeam.Id;

                return;
            }

            IDictionary<int, Ship> masterShips = m_currentState.Teams.SelectMany(t => t.Ships).ToDictionary(s => s.Id);
            IDictionary<int, Team> masterTeams = m_currentState.Teams.ToDictionary(t => t.Id);
            IDictionary<int, CtfGameState> updates = new Dictionary<int, CtfGameState>();
            HashSet<int> deadProjectiles = new HashSet<int>();

            //Resolve collisions
            foreach (var team in masterTeams.Values)
            {
                foreach (var projectile in team.Projectiles)
                {
                    if (deadProjectiles.Contains(projectile.Id))
                    {
                        //Check if this projectile was previously hit by another projectile
                        continue;
                    }

                    //Remove projectiles that go out of bounts
                    if (projectile.Position.Length() > Rules.ArenaRadius)
                    {
                        deadProjectiles.Add(projectile.Id);
                        continue;
                    }

                    foreach (var enemyTeam in masterTeams.Values)
                    {
                        if (team.Id == enemyTeam.Id)
                        {
                            continue;
                        }

                        foreach (var enemyShip in enemyTeam.Ships)
                        {
                            if (enemyShip.Bounds.Intersects(projectile.Bounds))
                            {
                                string killLog = string.Format("{0}'s ship ({1}) was killed by {2}'s ship ({3})", enemyTeam.Name, enemyShip.Id, team.Name, masterShips[projectile.FiredBy].Id);
                                m_currentState.Events.Add(killLog);

                                //Ship hit
                                deadProjectiles.Add(projectile.Id);
                                enemyShip.Position = SpawnPositions[enemyShip.Id];
                                team.Kills++;

                                //Reset flag position if the killed ship was holding a flag
                                foreach (var t in masterTeams.Values)
                                {
                                    if (t.Flag.HeldBy == enemyShip.Id)
                                    {
                                        string dropLog = string.Format("{0}'s flag was dropped by {1}'s ship ({2}) after it was killed", masterTeams[t.Flag.Owner].Name, enemyTeam.Name, enemyShip.Id);
                                        m_currentState.Events.Add(dropLog);
                                        t.Flag.HeldBy = null;
                                        t.Flag.Position = SpawnPositions[team.Flag.Id];
                                    }
                                }

                                continue;
                            }
                        }

                        if (deadProjectiles.Contains(projectile.Id))
                        {
                            continue;
                        }

                        foreach (var enemyProjectile in enemyTeam.Projectiles)
                        {
                            //check projectile vs projectile
                            if (enemyProjectile.Bounds.Intersects(projectile.Bounds))
                            {
                                string collideLog = string.Format("{0}'s projectile ({1}) collided with {2}'s projectile ({3})", team.Name, projectile.Id, enemyTeam.Name, enemyProjectile.Id);
                                m_currentState.Events.Add(collideLog);

                                deadProjectiles.Add(enemyProjectile.Id);
                                deadProjectiles.Add(projectile.Id);

                                
                                continue;
                            }
                        }

                        if (deadProjectiles.Contains(projectile.Id))
                        {
                            continue;
                        }
                    }

                    if (deadProjectiles.Contains(projectile.Id))
                    {
                        continue;
                    }
                }

                //Remove dead projectiles
                team.Projectiles = team.Projectiles.Where(p => !deadProjectiles.Contains(p.Id)).ToList();
            }

            //Check flag captures
            foreach (var team in masterTeams.Values)
            {
                //If the team's flag is captured...
                if (team.Flag.HeldBy != null)
                {
                    var holder = masterShips[team.Flag.HeldBy.Value];

                    team.Flag.Position = holder.Position;

                    var holderTeam = masterTeams[holder.Owner];
                    //If the flag's captor has brought the flag back to their team's base and their flag is there...
                    if (holderTeam.Flag.HeldBy == null && holderTeam.Flag.Bounds.Intersects(team.Flag.Bounds))
                    {
                        string capLog = string.Format("{0}'s flag has been captured by {1}'s ship ({2})", team.Name, holderTeam.Name, holder.Id);
                        m_currentState.Events.Add(capLog);

                        //Flag is captured!
                        holderTeam.FlagCaptures++;

                        //Reset flag
                        team.Flag.HeldBy = null;
                        team.Flag.Position = SpawnPositions[team.Flag.Id];
                    }
                }
                else
                {
                    //If team's flag isn't captured...
                    foreach (var enemyTeam in masterTeams.Values)
                    {
                        if (enemyTeam.Id == team.Id)
                        {
                            continue;
                        }

                        //Check if any enemy ships are intersecting it
                        foreach (var ship in enemyTeam.Ships)
                        {
                            //Enemy pickup flag
                            if (ship.Bounds.Intersects(team.Flag.Bounds))
                            {
                                string pickupLog = string.Format("{0}'s flag has been taken by {1}'s ship ({2})", team.Name, enemyTeam.Name, ship.Id);
                                m_currentState.Events.Add(pickupLog);

                                team.Flag.HeldBy = ship.Id;
                                break;
                            }
                        }

                        if (team.Flag.HeldBy != null)
                        {
                            break;
                        }
                    }
                }
            }

            //AI Act
            foreach (var ai in m_ais)
            {
                var aiStateCopy = m_currentState.Clone();

                try
                {
                    //Pass game state to each AI
                    ai.Value.Update(aiStateCopy, aiStateCopy.Teams.First(t => t.Id == ai.Key), aiStateCopy.Teams.Where(t => t.Id != ai.Key).ToList());
                }
                catch
                {
                }

                updates[ai.Key] = aiStateCopy;
            }

            //Apply updates
            foreach (var update in updates)
            {
                var aiStateCopy = update.Value;
                var aiId = update.Key;

                var team = aiStateCopy.Teams.FirstOrDefault(t => t.Id == aiId);
                if (team != null)
                {
                    if (team.Ships != null)
                    {
                        foreach (var updatedShip in team.Ships)
                        {
                            Ship masterShip = null;
                            if (masterShips.TryGetValue(updatedShip.Id, out masterShip))
                            {
                                if (masterShip.Owner == aiId)
                                {
                                    //Copy across updates from cloned state
                                    masterShip.SetLabel(updatedShip.Label);
                                    masterShip.SetThrust(updatedShip.Thrust);
                                    masterShip.SetTorque(updatedShip.Torque);
                                    if (updatedShip.IsFiring)
                                    {
                                        masterShip.Fire();
                                    }
                                    else
                                    {
                                        masterShip.StopFiring();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Step firing
            foreach (var ship in masterShips.Values)
            {
                if (ship.IsFiring)
                {
                    int lastFireTurn = m_lastFireTurns[ship.Id];

                    //Check the firing isn't on cooldown
                    if (m_currentState.TurnNumber - lastFireTurn > Rules.FireCooldown)
                    {
                        m_lastFireTurns[ship.Id] = m_currentState.TurnNumber;

                        var team = masterTeams[ship.Owner];

                        //Fire a projectile in the direction the ship is facing
                        team.Projectiles.Add(new Projectile()
                        {
                            Bounds = new BoundingCircle(ship.Position, Rules.ProjectileRadius),
                            FiredBy = ship.Id,
                            Owner = team.Id,
                            Velocity = Vector2.FromPolar(Rules.ProjectileVelocity, ship.Rotation)
                        });
                    }
                }
            }

            //Step physics
            m_currentState = Step(m_currentState);
            States.Add(m_currentState);
        }

        private CtfGameState Step(CtfGameState state)
        {
            CtfGameState newState = state.Clone();

            newState.TurnNumber++;

            foreach (var team in newState.Teams)
            {
                StepTeam(team);
            }

            return newState;
        }

        private void StepTeam(Team team)
        {
            foreach (var ship in team.Ships)
            {
                StepShip(ship);
            }

            foreach (var projectile in team.Projectiles)
            {
                StepProjectile(projectile);
            }
        }

        private void StepShip(Ship ship)
        {
            ship.Rotation += ship.AngularVelocity;
            ship.Torque = MathHelper.Clamp(ship.Torque, -1f, 1f);
            ship.AngularVelocity += ship.Torque * Rules.TorquePower;
            ship.AngularVelocity = MathHelper.Clamp(ship.AngularVelocity, -Rules.MaxShipAngularVelocity, Rules.MaxShipAngularVelocity);

            ship.Position += ship.Velocity;
            ship.Thrust = MathHelper.Clamp(ship.Thrust, -1f, 1f);
            float thrustPower = ship.Thrust * Rules.ThrustPower;
            ship.Velocity += Vector2.FromPolar(thrustPower, ship.Rotation);

            //Clamp velocity
            if (ship.Velocity.Length() > Rules.MaxShipVelocity)
            {
                ship.Velocity = Vector2.Normalize(ship.Velocity) * Rules.MaxShipVelocity;
            }

            //Reflect velocity if out of bounds
            if (ship.Position.Length() >= Rules.ArenaRadius)
            {
                ship.Velocity = Vector2.Reflect(ship.Velocity, -Vector2.Normalize(ship.Position));
            }
        }

        private void StepProjectile(Projectile projectile)
        {
            projectile.Position += projectile.Velocity;
        }
    }
}