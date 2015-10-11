using Newtonsoft.Json;

namespace AiCtf.Sdk
{
    /// <summary>
    /// The state of a game entity
    /// </summary>
    public class GameEntity
    {
        /// <summary>
        /// The unique id of this entity
        /// </summary>
        [JsonProperty]
        public int Id { get; internal set; }

        /// <summary>
        /// The id of the team that owns this game entity
        /// </summary>
        [JsonProperty]
        public int Owner { get; internal set; }

        /// <summary>
        /// The bounds of this game entity
        /// </summary>
        [JsonProperty]
        public BoundingCircle Bounds { get; internal set; }

        /// <summary>
        /// The velocity of this game entity
        /// </summary>
        [JsonProperty]
        public Vector2 Velocity { get; internal set; }

        /// <summary>
        /// The position of this game entity
        /// </summary>
        [JsonIgnore]
        public Vector2 Position
        {
            get { return Bounds.Center; }
            internal set { Bounds = new BoundingCircle(value, Bounds.Radius); }
        }

        /// <summary>
        /// Create a new game entity with a unique id
        /// </summary>
        public GameEntity()
        {
            Id = CtfId.NewId();
        }
    }
}