using AiCtf.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace AiCtf
{
    class Program
    {
        static void Main(string[] args)
        {
            CtfGame game = new CtfGame(new CtfGameRules()
            {
                ArenaRadius = 512,
                FireCooldown = 100,
                FlagRadius = 32,
                MaxShipAngularVelocity = 0.5f,
                MaxShipVelocity = 16,
                ProjectileRadius = 8,
                ProjectileVelocity = 64,
                ShipRadius = 32,
                ShipsPerTeam = 5,
                ThrustPower = 0.5f,
                TorquePower = 0.01f,
                TurnLimit = 500,
                FlagLimit = 3
            },
            new List<ICtfAi>() { new TestAi() { Name = "One" }, new TestAi() { Name = "Two " } });

            game.Initialise();

            while (game.WinningTeam == null)
            {
                game.Step();
            }

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

            public void Initialize(CtfGameRules rules, Guid yourTeamId)
            {
            }

            public void Update(CtfGameState state, IList<Ship> yourShips)
            {
                foreach (var s in yourShips)
                {
                    s.Fire();
                    s.SetThrust((float)((m_rand.NextDouble() * 2) - 1));
                    s.SetTorque((float)((m_rand.NextDouble() * 2) - 1));
                }
            }
        }
    }
}
