using Newtonsoft.Json;

namespace AiCtf.Sdk
{
    /// <summary>
    /// The state of a projectile
    /// </summary>
    public class Projectile : GameEntity
    {
        /// <summary>
        /// The id of the ship that fired this projectile
        /// </summary>
        [JsonProperty]
        public int FiredBy { get; internal set; }

        /// <summary>
        /// Clone this projectile
        /// </summary>
        public Projectile Clone()
        {
            return new Projectile()
            {
                Bounds = Bounds,
                FiredBy = FiredBy,
                Id = Id,
                Owner = Owner,
                Position = Position,
                Velocity = Velocity
            };
        }
    }
}