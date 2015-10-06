using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCtf.Sdk
{
    public interface ICtfAi
    {
        string Name { get; }
        void Initialize(CtfGameRules rules, Guid yourTeamId);
        void Update(CtfGameState state, IList<Ship> yourShips);
    }
}
