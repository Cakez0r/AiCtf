using Newtonsoft.Json;

namespace AiCtf.Sdk
{
    /// <summary>
    /// The state of a flag
    /// </summary>
    public class Flag : GameEntity
    {
        /// <summary>
        /// The id of the ship that is currently holding this flag. If the flag is currently not captured, this id will be null.
        /// </summary>
        [JsonProperty]
        public int? HeldBy { get; internal set; }

        /// <summary>
        /// Clone this flag
        /// </summary>
        public Flag Clone()
        {
            return new Flag()
            {
                Bounds = Bounds,
                HeldBy = HeldBy,
                Id = Id,
                Owner = Owner,
                Position = Position,
                Velocity = Velocity
            };
        }
    }
}