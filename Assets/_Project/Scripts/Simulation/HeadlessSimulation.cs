#if HEADLESS_SIM
using UnityEngine;
using UnityFoundation.Core;
using UnityFoundation.Services;

namespace UnityFoundation.Simulation
{
    public static class HeadlessSimulation
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Run() => RunSimulation();

        public static void RunSimulation()
        {
            Debug.Log("[Headless] Bootstrapping services...");

            ServiceLocator.Register<ICrashReporter>(new CrashReporter());
            ServiceLocator.Register<ILogService>(new LogService());
            ServiceLocator.Register<IEventBus>(new EventBus());
            ServiceLocator.Register<IGameStateManager>(new GameStateManager());

            ServiceLocator.Get<ICrashReporter>().Initialize();
            ServiceLocator.Get<ILogService>().Initialize();
            ServiceLocator.Get<IEventBus>().Initialize();
            ServiceLocator.Get<IGameStateManager>().Initialize();

            Debug.Log("[Headless] Services ready. Starting simulation session...");

            var gsm = ServiceLocator.Get<IGameStateManager>();

            "[Headless] → TransitionTo(MainMenu)".Log(LogCategory.Bootstrap);
            gsm.TransitionTo(GameState.MainMenu);

            "[Headless] → TransitionTo(Gameplay)".Log(LogCategory.Bootstrap);
            gsm.TransitionTo(GameState.Gameplay);

            "[Headless] → TransitionTo(Paused)".Log(LogCategory.Bootstrap);
            gsm.TransitionTo(GameState.Paused);

            "[Headless] → TransitionTo(Gameplay)".Log(LogCategory.Bootstrap);
            gsm.TransitionTo(GameState.Gameplay);

            "[Headless] → TransitionTo(GameOver)".Log(LogCategory.Bootstrap);
            gsm.TransitionTo(GameState.GameOver);

            Debug.Log("[Headless] Simulation complete. Clearing services.");
            ServiceLocator.Clear();
        }
    }
}
#endif
