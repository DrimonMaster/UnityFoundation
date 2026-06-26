using System;
using System.Collections.Generic;

namespace UnityFoundation.Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> _services = new();

        public static void Register<T>(T service) where T : IService
        {
            _services[typeof(T)] = service;
        }

        public static T Get<T>() where T : IService
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;
            throw new InvalidOperationException($"Service {typeof(T).Name} is not registered.");
        }

        public static bool TryGet<T>(out T service) where T : IService
        {
            if (_services.TryGetValue(typeof(T), out var s))
            {
                service = (T)s;
                return true;
            }
            service = default;
            return false;
        }

        public static void Unregister<T>() where T : IService
        {
            _services.Remove(typeof(T));
        }

        // Disposes all services and clears the registry. Call from GameBootstrap.OnDestroy.
        public static void Clear()
        {
            foreach (var service in _services.Values)
                service.Dispose();
            _services.Clear();
        }
    }
}