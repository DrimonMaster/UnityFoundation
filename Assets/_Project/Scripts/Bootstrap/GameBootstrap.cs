using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityFoundation.Core;
using UnityFoundation.Services;

namespace UnityFoundation.Bootstrap
{
    public class GameBootstrap : MonoBehaviour
    {
        private readonly List<IService> _services = new();

        private async void Start()
        {
            try
            {
                "Registering services...".Log(LogCategory.Bootstrap);
                RegisterServices();
                "Initializing all services...".Log(LogCategory.Bootstrap);
                InitializeAll();
                "Starting async sequence...".Log(LogCategory.Bootstrap);
                await RunStartupSequence();
                "Loading MainMenu...".Log(LogCategory.Bootstrap);
                SceneManager.LoadScene("MainMenu");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void OnDestroy() => ServiceLocator.Clear();

        private void RegisterServices()
        {
            Add<ICrashReporter>(new CrashReporter());
            Add<ILogService>(new LogService());
            Add<IEventBus>(new EventBus());
            Add<IDataService>(new JsonDataService());
            Add<IGameStateManager>(new GameStateManager());
            Add<INetworkService>(new NetworkService());
            Add<IUIService>(new UIService());
            Add<ILoadingService>(new LoadingService());
        }

        private void Add<T>(T service) where T : IService
        {
            ServiceLocator.Register(service);
            _services.Add(service);
        }

        private void InitializeAll()
        {
            foreach (var service in _services)
                service.Initialize();
        }

        private async Task RunStartupSequence()
        {
            // Fire all async services simultaneously
            var asyncTasks = _services
                .OfType<IAsyncService>()
                .ToDictionary(s => s, s => s.StartAsync());

            // Block until all Critical async services are ready (no loader yet)
            await Task.WhenAll(asyncTasks
                .Where(kv => kv.Key.Priority == InitPriority.Critical)
                .Select(kv => kv.Value));

            "All critical services ready".Log(LogCategory.Bootstrap);

            // Show loader while Important services finish
            ServiceLocator.Get<ILoadingService>().Show();

            await Task.WhenAll(asyncTasks
                .Where(kv => kv.Key.Priority == InitPriority.Important)
                .Select(kv => kv.Value));

            ServiceLocator.Get<ILoadingService>().Hide();

            // Optional tasks already running in background — just wire up error logging
            foreach (var task in asyncTasks
                .Where(kv => kv.Key.Priority == InitPriority.Optional)
                .Select(kv => kv.Value))
            {
                _ = task.ContinueWith(
                    t => Debug.LogException(t.Exception),
                    TaskContinuationOptions.OnlyOnFaulted);
            }
        }
    }
}
