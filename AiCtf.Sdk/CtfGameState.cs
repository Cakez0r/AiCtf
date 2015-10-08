using System.Collections.Generic;
using System.Linq;

namespace AiCtf.Sdk
{
    public class CtfGameState
    {
        public int TurnNumber { get; set; }
        public IList<Team> Teams { get; set; }

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