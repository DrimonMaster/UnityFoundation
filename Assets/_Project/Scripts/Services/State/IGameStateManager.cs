using System;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface IGameStateManager : IService
    {
        GameState CurrentState { get; }
        event Action<GameState, GameState> OnStateChanged;
        void TransitionTo(GameState newState);
    }
}
