using Newtonsoft.Json;

namespace AiCtf.Sdk
{
    /// <summary>
    /// The state of a ship
    /// </summary>
    public class Ship : GameEntity
    {
        /// <summary>
        /// The debug label of this ship
        /// </summary>
        [JsonProperty]
        public string Label { get; internal set; }

        /// <summary>
        /// The current thruster power of this ship. Valid range is between [-1 .. 1]
        /// </summary>
        [JsonProperty]
        public float Thrust { get; internal set; }

        /// <summary>
        /// The current torque power of this ship. Valid range is between [-1 .. 1]
        /// </summary>
        [JsonProperty]
        public float Torque { get; internal set; }

        /// <summary>
        /// Whether this ship is firing
        /// </summary>
        [JsonProperty]
        public bool IsFiring { get; internal set; }

        /// <summary>
        /// The current rotation of this ship
        /// </summary>
        [JsonProperty]
        public float Rotation { get; internal set; }

        /// <summary>
        /// This current angular velocity of this ship
        /// </summary>
        [JsonProperty]
        public float AngularVelocity { get; internal set; }

        /// <summary>
        /// Clone this ship
        /// </summary>
        public Ship Clone()
        {
            return new Ship()
            {
                AngularVelocity = AngularVelocity,
                Bounds = Bounds,
                Id = Id,
                Label = Label,
                Owner = Owner,
                Position = Position,
                Rotation = Rotation,
                Thrust = Thrust,
                Torque = Torque,
                Velocity = Velocity
            };
        }

        /// <summary>
        /// Set the debug label of this ship. Maximum length is 128 characters
        /// </summary>
        public void SetLabel(string label)
        {
            Label = label ?? string.Empty;
            if (Label.Length > 128)
            {
                Label = Label.Substring(0, 128);
            }
        }

        /// <summary>
        /// Set the current thrust power of this ship. Valid range is between [-1 .. 1]
        /// </summary>
        public void SetThrust(float thrust)
        {
            Thrust = MathHelper.Clamp(thrust, -1f, 1f);
        }

        /// <summary>
        /// Set the current torque power of this ship. Valid range is between [-1 .. 1]
        /// </summary>
        public void SetTorque(float torque)
        {
            Torque = MathHelper.Clamp(torque, -1f, 1f);
        }

        /// <summary>
        /// Start firing the gun of this ship. 
        /// While the ship is firing, projectiles will be fired as often as the game rules permit.
        /// </summary>
        public void Fire()
        {
            IsFiring = true;
        }

        /// <summary>
        /// Stop firing the gun of this ship.
        /// </summary>
        public void StopFiring()
        {
            IsFiring = false;
        }
    }
}