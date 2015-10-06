using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCtf.Sdk
{
    public class CtfGameRules
    {
        public float ArenaRadius { get; set; }
        public float ShipRadius { get; set; }
        public float FlagRadius { get; set; }
        public float ProjectileRadius { get; set; }

        public float MaxShipVelocity { get; set; }
        public float MaxShipAngularVelocity { get; set; }

        public float ProjectileVelocity { get; set; }

        public float ThrustPower { get; set; }
        public float TorquePower { get; set; }

        public int TurnLimit { get; set; }

        public int FireCooldown { get; set; }

        public int ShipsPerTeam { get; set; }
    }
}