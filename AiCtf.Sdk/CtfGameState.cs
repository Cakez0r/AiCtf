using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AiCtf.Sdk
{
    /// <summary>
    /// The state of one turn in a ctf game
    /// </summary>
    public class CtfGameState
    {
        /// <summary>
        /// The number of this turn
        /// </summary>
        [JsonProperty]
        public int TurnNumber { get; internal set; }

        /// <summary>
        /// The state of each team in this game
        /// </summary>
        [JsonProperty]
        public IList<Team> Teams { get; internal set; }

        /// <summary>
        /// Any game events that have occurred this turn
        /// </summary>
        [JsonProperty]
        public IList<string> Events { get; internal set; }

        /// <summary>
        /// Initialise a turn state
        /// </summary>
        public CtfGameState()
        {
            Events = new List<string>();
            Teams = new List<Team>();
        }

        /// <summary>
        /// Clone this game state
        /// </summary>
        public CtfGameState Clone()
        {
            return new CtfGameState()
            {
                TurnNumber = TurnNumber,
                Teams = Teams.Select(t => t.Clone()).ToList()
            };
        }
    }
}