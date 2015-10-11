namespace AiCtf.Sdk
{
    /// <summary>
    /// The parameters of a capture the flag game
    /// </summary>
    public class CtfGameRules
    {
        /// <summary>
        /// The radius of the battle arena
        /// </summary>
        public float ArenaRadius { get; set; }

        /// <summary>
        /// The radius of each ship
        /// </summary>
        public float ShipRadius { get; set; }

        /// <summary>
        /// The radius of each flag
        /// </summary>
        public float FlagRadius { get; set; }

        /// <summary>
        /// The radius of each projectile
        /// </summary>
        public float ProjectileRadius { get; set; }

        /// <summary>
        /// The maximum velocity of each ship
        /// </summary>
        public float MaxShipVelocity { get; set; }

        /// <summary>
        /// The maximum angular velocity of each ship
        /// </summary>
        public float MaxShipAngularVelocity { get; set; }

        /// <summary>
        /// The maximum velocity of each projectile
        /// </summary>
        public float ProjectileVelocity { get; set; }

        /// <summary>
        /// The maximum amount of velocity that can be added per turn
        /// </summary>
        public float ThrustPower { get; set; }

        /// <summary>
        /// The maximum amount of angular velocity that can be added per turn
        /// </summary>
        public float TorquePower { get; set; }

        /// <summary>
        /// The maximum amount of turns that are allowed in a game
        /// </summary>
        public int TurnLimit { get; set; }

        /// <summary>
        /// The maximum amount of flag captures allowed in a game
        /// </summary>
        public int FlagLimit { get; set; }

        /// <summary>
        /// The minimum number of turns that must elapse before two subsequent shots can be fired from each ship
        /// </summary>
        public int FireCooldown { get; set; }

        /// <summary>
        /// The number of ships on each team
        /// </summary>
        public int ShipsPerTeam { get; set; }

        /// <summary>
        /// Initialise with the default game rules
        /// </summary>
        public CtfGameRules()
        {
            ArenaRadius = 512;
            FireCooldown = 100;
            FlagRadius = 32;
            MaxShipAngularVelocity = 0.1f;
            MaxShipVelocity = 5;
            ProjectileRadius = 8;
            ProjectileVelocity = 10;
            ShipRadius = 32;
            ShipsPerTeam = 5;
            ThrustPower = 0.25f;
            TorquePower = 0.01f;
            TurnLimit = 9000;
            FlagLimit = 3;
        }

        public CtfGameRules Clone()
        {
            return new CtfGameRules()
            {
                ArenaRadius = ArenaRadius,
                FireCooldown = FireCooldown,
                FlagLimit = FireCooldown,
                FlagRadius = FlagRadius,
                MaxShipAngularVelocity = MaxShipAngularVelocity,
                MaxShipVelocity = MaxShipVelocity,
                ProjectileRadius = ProjectileRadius,
                ProjectileVelocity = ProjectileRadius,
                ShipRadius = ShipRadius,
                ShipsPerTeam = ShipsPerTeam,
                ThrustPower = ThrustPower,
                TorquePower = TorquePower,
                TurnLimit = TurnLimit
            };
        }
    }
}