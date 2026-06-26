using System;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class GameStateManager : IGameStateManager
    {
        public InitPriority Priority => InitPriority.Critical;
        public bool IsReady { get; private set; }
        public GameState CurrentState { get; private set; }
        public event Action<GameState> OnStateChanged;

        public void Initialize()
        {
            CurrentState = GameState.Menu;
            IsReady = true;
            "[Lifecycle] GameStateManager initialized".Log(LogCategory.Lifecycle);
        }

        public void Dispose()
        {
            "[Lifecycle] GameStateManager disposed".Log(LogCategory.Lifecycle);
            IsReady = false;
        }

        public void TransitionTo(GameState state)
        {
            CurrentState = state;
            OnStateChanged?.Invoke(state);
        }
    }
}
