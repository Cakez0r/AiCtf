using System.Collections.Generic;

namespace AiCtf.Sdk
{
    /// <summary>
    /// The interface that must be implemented to control an AiCtf team
    /// </summary>
    public interface ICtfAi
    {
        /// <summary>
        /// The name of this AI
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Initialise this AI
        /// </summary>
        /// <param name="rules">The parameters of the game that this AI will be participating in</param>
        /// <param name="teamId">The id of the team that this AI will be controlling</param>
        void Initialize(CtfGameRules rules, int teamId);

        /// <summary>
        /// Update the team that this AI controls. This will be called every turn
        /// </summary>
        /// <param name="state">The current state of the game</param>
        /// <param name="yourTeam">The team that this AI controls</param>
        /// <param name="enemyTeams">All enemy teams within the game</param>
        void Update(CtfGameState state, Team yourTeam, IList<Team> enemyTeams);
    }
}