using System;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface IGameStateManager : IService
    {
        GameState CurrentState { get; }
        event Action<GameState> OnStateChanged;
        void TransitionTo(GameState state);
    }
}
