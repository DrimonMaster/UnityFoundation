using System;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class GameStateManager : IGameStateManager
    {
        private IEventBus _eventBus;

        public InitPriority Priority => InitPriority.Critical;
        public bool IsReady { get; private set; }
        public GameState CurrentState { get; private set; }
        public event Action<GameState, GameState> OnStateChanged;

        public void Initialize()
        {
            _eventBus = ServiceLocator.Get<IEventBus>();
            CurrentState = GameState.MainMenu;
            IsReady = true;
            "[Lifecycle] GameStateManager initialized".Log(LogCategory.Lifecycle);
        }

        public void Dispose()
        {
            "[Lifecycle] GameStateManager disposed".Log(LogCategory.Lifecycle);
            IsReady = false;
        }

        public void TransitionTo(GameState newState)
        {
            if (newState == CurrentState)
            {
                $"GameState: transition to {newState} ignored (already in this state)".Log(LogCategory.GameState);
                return;
            }

            var from = CurrentState;
            CurrentState = newState;

            $"GameState: {from} → {newState}".Log(LogCategory.GameState);
            _eventBus.Publish(new GameStateChangedEvent(from, newState));
            OnStateChanged?.Invoke(from, newState);
        }
    }
}
