using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AiCtf.Sdk
{
    /// <summary>
    /// The state of a team
    /// </summary>
    public class Team
    {
        /// <summary>
        /// The unique id of this team
        /// </summary>
        [JsonProperty]
        public int Id { get; internal set; }

        /// <summary>
        /// The name of this team
        /// </summary>
        [JsonProperty]
        public string Name { get; internal set; }

        /// <summary>
        /// This team's flag
        /// </summary>
        [JsonProperty]
        public Flag Flag { get; internal set; }

        /// <summary>
        /// All ships that this team controls
        /// </summary>
        [JsonProperty]
        public IList<Ship> Ships { get; internal set; }

        /// <summary>
        /// All projectiles that have been fired by ships that this team owns
        /// </summary>
        [JsonProperty]
        public IList<Projectile> Projectiles { get; internal set; }

        /// <summary>
        /// The number of times this team has captured a flag
        /// </summary>
        [JsonProperty]
        public int FlagCaptures { get; internal set; }

        /// <summary>
        /// The number of enemy ships that this team has killed
        /// </summary>
        [JsonProperty]
        public int Kills { get; internal set; }

        /// <summary>
        /// Initialise this team with a unique id
        /// </summary>
        public Team()
        {
            Id = CtfId.NewId();
        }

        /// <summary>
        /// Clone this team
        /// </summary>
        public Team Clone()
        {
            return new Team()
            {
                Id = Id,
                Name = Name,
                Flag = Flag.Clone(),
                Ships = Ships.Select(s => s.Clone()).ToList(),
                Projectiles = Projectiles.Select(p => p.Clone()).ToList(),
                FlagCaptures = FlagCaptures,
                Kills = Kills
            };
        }
    }
}