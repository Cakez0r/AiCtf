using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AiCtf.Sdk
{
    public class CtfGame
    {
        [JsonProperty]
        public IList<CtfGameState> States { get; private set; }

        [JsonProperty]
        public CtfGameRules Rules { get; private set; }

        [JsonProperty]
        public Dictionary<Guid, Vector2> SpawnPositions { get; private set; }

        [JsonProperty]
        public Guid? WinningTeam { get; private set; }

        private Dictionary<Guid, int> m_lastFireTurns = new Dictionary<Guid, int>();
        private Dictionary<Guid, ICtfAi> m_ais = new Dictionary<Guid, ICtfAi>();
        private CtfGameState m_currentState;

        [JsonConstructor]
        internal CtfGame()
        {

        }

        public CtfGame(CtfGameRules rules, IList<ICtfAi> ais)
        {
            SpawnPositions = new Dictionary<Guid, Vector2>();
            Rules = rules;
            States = new List<CtfGameState>(Rules.TurnLimit);

            float teamSpacing = MathHelper.TwoPi / ais.Count;
            float shipSpacing = MathHelper.TwoPi / Rules.ShipsPerTeam;

            float flagRadius = Rules.ArenaRadius - (2 * Rules.ShipRadius) - Rules.FlagRadius;

            CtfGameState initialState = new CtfGameState();

            initialState.Teams = new List<Team>(ais.Count);
            int teamIndex = 0;

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
                        Rotation = (i * shipSpacing) + MathHelper.Pi,
                    };

                    SpawnPositions[ship.Id] = ship.Position;
                    m_lastFireTurns[ship.Id] = int.MinValue;

                    team.Ships.Add(ship);
                }

                team.Projectiles = new List<Projectile>();

                initialState.Teams.Add(team);

                teamIndex++;
            }

            States.Add(initialState);
            m_currentState = initialState;
        }

        public void Initialise()
        {
            foreach (var ai in m_ais)
            {
                ai.Value.Initialize(Rules, ai.Key);
            }
        }

        public void Step()
        {
            if (States.Count == Rules.TurnLimit || m_currentState.Teams.Any(t => t.FlagCaptures == Rules.FlagLimit))
            {
                var winningTeam = m_currentState.Teams.OrderByDescending(t => t.FlagCaptures).ThenByDescending(t => t.Kills).FirstOrDefault();
                WinningTeam = winningTeam.Id;
                return;
            }

            IDictionary<Guid, Ship> masterShips = m_currentState.Teams.SelectMany(t => t.Ships).ToDictionary(s => s.Id);
            IDictionary<Guid, Team> masterTeams = m_currentState.Teams.ToDictionary(t => t.Id);
            IDictionary<Guid, CtfGameState> updates = new Dictionary<Guid, CtfGameState>();
            HashSet<Guid> deadProjectiles = new HashSet<Guid>();

            //Resolve collisions
            foreach (var team in masterTeams.Values)
            {
                foreach (var projectile in team.Projectiles)
                {
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
                                //Ship hit
                                deadProjectiles.Add(projectile.Id);
                                enemyShip.Position = SpawnPositions[enemyShip.Id];
                                team.Kills++;

                                //Reset flag position if the killed ship was holding a flag
                                foreach (var t in masterTeams.Values)
                                {
                                    if (t.Flag.HeldBy == enemyShip.Id)
                                    {
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
                    }

                    if (deadProjectiles.Contains(projectile.Id))
                    {
                        continue;
                    }
                }

                team.Projectiles = team.Projectiles.Where(p => !deadProjectiles.Contains(p.Id)).ToList();
            }

            foreach (var team in masterTeams.Values)
            {
                if (team.Flag.HeldBy != null)
                {
                    var holder = masterShips[team.Flag.HeldBy.Value];

                    team.Flag.Position = holder.Position;

                    var holderTeam = masterTeams[holder.Owner];
                    if (holderTeam.Flag.HeldBy == null && holderTeam.Flag.Bounds.Intersects(team.Flag.Bounds))
                    {
                        holderTeam.FlagCaptures++;

                        team.Flag.HeldBy = null;
                        team.Flag.Position = SpawnPositions[team.Flag.Id];
                    }
                }
                else
                {
                    foreach (var enemyTeam in masterTeams.Values)
                    {
                        if (enemyTeam.Id == team.Id)
                        {
                            continue;
                        }

                        foreach (var ship in enemyTeam.Ships)
                        {
                            if (ship.Bounds.Intersects(team.Flag.Bounds))
                            {
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
                ai.Value.Update(aiStateCopy, aiStateCopy.Teams.First(t => t.Id == ai.Key).Ships);
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

                    if (m_currentState.TurnNumber - lastFireTurn > Rules.FireCooldown)
                    {
                        m_lastFireTurns[ship.Id] = m_currentState.TurnNumber;

                        var team = masterTeams[ship.Owner];
                        team.Projectiles.Add(new Projectile()
                        {
                            AngularVelocity = 0,
                            Bounds = new BoundingCircle(ship.Position, Rules.ProjectileRadius),
                            FiredBy = ship.Id,
                            Owner = team.Id,
                            Rotation = 0,
                            Velocity = new Vector2(Rules.ProjectileVelocity * (float)Math.Cos(ship.Rotation), Rules.ProjectileVelocity * (float)Math.Sin(ship.Rotation))
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
            ship.Velocity += new Vector2(thrustPower * (float)Math.Cos(ship.Rotation), thrustPower * (float)Math.Sin(ship.Rotation));

            if (ship.Velocity.Length() > Rules.MaxShipVelocity)
            {
                ship.Velocity = Vector2.Normalize(ship.Velocity) * Rules.MaxShipVelocity;
            }

            if (ship.Position.Length() > Rules.ArenaRadius)
            {
                ship.Position = Vector2.Normalize(ship.Position) * Rules.ArenaRadius;
            }
        }

        private void StepProjectile(Projectile projectile)
        {
            projectile.Velocity = new Vector2(Rules.ProjectileVelocity * (float)Math.Cos(projectile.Rotation), Rules.ProjectileVelocity * (float)Math.Sin(projectile.Rotation));
            projectile.Position += projectile.Velocity;
        }
    }
}