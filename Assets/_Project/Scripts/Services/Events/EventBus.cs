using System;
using System.Collections.Generic;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<HandlerEntry>> _handlers = new();
        private readonly object _lock = new();

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
            lock (_lock) _handlers.Clear();
            IsReady = false;
        }

        public IDisposable Subscribe<T>(Action<T> handler, int priority = 0)
        {
            lock (_lock)
            {
                if (!_handlers.TryGetValue(typeof(T), out var list))
                {
                    list = new List<HandlerEntry>();
                    _handlers[typeof(T)] = list;
                }
                list.Add(new HandlerEntry { Handler = handler, Priority = priority });
                list.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            }
            return new SubscriptionToken(() => Unsubscribe<T>(handler));
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            lock (_lock)
            {
                if (_handlers.TryGetValue(typeof(T), out var list))
                    list.RemoveAll(e => e.Handler == (Delegate)handler);
            }
        }

        public void Publish<T>(T eventData)
        {
            List<HandlerEntry> snapshot;
            lock (_lock)
            {
                if (!_handlers.TryGetValue(typeof(T), out var list) || list.Count == 0) return;
                $"[Event] {typeof(T).Name} → {list.Count} subscribers".Log(LogCategory.Event);
                snapshot = new List<HandlerEntry>(list);
            }
            foreach (var entry in snapshot)
                ((Action<T>)entry.Handler).Invoke(eventData);
        }

        public IDisposable SubscribeOnce<T>(Action<T> handler, int priority = 0)
        {
            Action<T> wrapper = null;
            wrapper = evt =>
            {
                Unsubscribe<T>(wrapper);
                handler(evt);
            };
            return Subscribe<T>(wrapper, priority);
        }

        private struct HandlerEntry
        {
            public Delegate Handler;
            public int Priority;
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
