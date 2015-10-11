using AiCtf.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AiCtf
{
    class Program
    {
        static void Main(string[] args)
        {
            CtfGame game = new CtfGame(new CtfGameRules(), new List<ICtfAi>()
            {
                new TestAi() { Name = "One" },
                new TestAi() { Name = "Two" }
            });

            game.Initialise();

            while (game.WinningTeam == null)
            {
                var state = game.States.Last();
                game.Step();

                foreach (var e in state.Events)
                {
                    Console.WriteLine(e);
                }
            }

            Console.ReadKey();

            File.Delete("game.json");
            File.WriteAllText("game.json", JsonConvert.SerializeObject(game));
        }

        private class TestAi : ICtfAi
        {
            private static Random m_rand = new Random();
            public string Name
            {
                get;
                set;
            }

            public void Initialize(CtfGameRules rules, int yourTeamId)
            {
            }

            public void Update(CtfGameState state, Team yourTeam, IList<Team> enemyTeams)
            {
                var enemy = enemyTeams[0];
                foreach (var ship in yourTeam.Ships)
                {
                    if (Math.Round(m_rand.NextDouble()) == 0)
                    {
                        ship.Fire();
                    }
                    else
                    {
                        ship.StopFiring();
                    }

                    ship.SetThrust((float)m_rand.NextDouble());

                    var t = enemy.Flag.HeldBy == ship.Id ? yourTeam.Flag.Position : yourTeam.Flag.HeldBy != null ? yourTeam.Flag.Position : enemy.Flag.Position;
                    Vector2 target = Vector2.Normalize(t - ship.Position);
                    if (!(float.IsNaN(target.X) && float.IsNaN(target.Y)))
                    {
                        float d = (float)Math.Atan2(target.Y, target.X) - (ship.Rotation + (ship.AngularVelocity * 5));
                        ship.SetTorque(Math.Sign(d));
                    }
                }
            }
        }
    }
}
