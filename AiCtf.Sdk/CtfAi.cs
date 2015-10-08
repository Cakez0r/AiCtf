using System;
using System.Collections.Generic;

namespace AiCtf.Sdk
{
    public interface ICtfAi
    {
        string Name { get; }
        void Initialize(CtfGameRules rules, Guid teamId);
        void Update(CtfGameState state, IList<Ship> ships);
    }
}
