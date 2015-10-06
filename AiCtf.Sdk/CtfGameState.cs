using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCtf.Sdk
{
    public class CtfGameState
    {
        public int TurnNumber { get; internal set; }
        public IList<Team> Teams { get; internal set; }

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