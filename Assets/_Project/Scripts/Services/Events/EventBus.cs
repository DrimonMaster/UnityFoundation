using System;
using System.Collections.Generic;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public InitPriority Priority => InitPriority.Critical;
        public bool IsReady { get; private set; }

        public void Initialize()
        {
            IsReady = true;
            "[Lifecycle] EventBus initialized".Log(LogCategory.Lifecycle);
        }

        public void Dispose()
        {
            "[Lifecycle] EventBus disposed".Log(LogCategory.Lifecycle);
            _handlers.Clear();
            IsReady = false;
        }

        public IDisposable Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _handlers[type] = list;
            }
            list.Add(handler);
            return new SubscriptionToken(() => Unsubscribe<T>(handler));
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            if (_handlers.TryGetValue(typeof(T), out var list))
                list.Remove(handler);
        }

        public void Publish<T>(T eventData)
        {
            $"Event published: {typeof(T).Name}".Log(LogCategory.Event);
            if (!_handlers.TryGetValue(typeof(T), out var list)) return;
            foreach (var handler in list.ToArray())
                ((Action<T>)handler).Invoke(eventData);
        }

        private sealed class SubscriptionToken : IDisposable
        {
            private readonly Action _unsubscribe;
            private bool _disposed;

            public SubscriptionToken(Action unsubscribe) => _unsubscribe = unsubscribe;

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                _unsubscribe();
            }
        }
    }
}
