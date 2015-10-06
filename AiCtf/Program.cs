using AiCtf.Sdk;
using System;
using System.Collections.Generic;

namespace AiCtf
{
    class Program
    {
        static void Main(string[] args)
        {
            CtfGame game = new CtfGame(new CtfGameRules()
            {
                ArenaRadius = 1024,
                FireCooldown = 100,
                FlagRadius = 32,
                MaxShipAngularVelocity = 32,
                MaxShipVelocity = 32,
                ProjectileRadius = 8,
                ProjectileVelocity = 64,
                ShipRadius = 32,
                ShipsPerTeam = 5,
                ThrustPower = 32,
                TorquePower = 32,
                TurnLimit = 100000000
            },
            new List<ICtfAi>() { new TestAi() { Name = "One" }, new TestAi() { Name = "Two " } });

            game.Initialise();

            for (int i = 0; i < 100; i++)
            {
                game.Step();
            }
        }

        private class TestAi : ICtfAi
        {
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
            }
        }
    }
}
