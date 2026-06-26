using System;
using System.Collections.Generic;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();

        public InitPriority Priority => InitPriority.Critical;
        public bool IsReady { get; private set; }

        public void Initialize() => IsReady = true;

        public void Dispose()
        {
            _handlers.Clear();
            IsReady = false;
        }

        public void Publish<T>(T evt) where T : struct
        {
            if (_handlers.TryGetValue(typeof(T), out var del))
                ((Action<T>)del)?.Invoke(evt);
        }

        public void Subscribe<T>(Action<T> handler) where T : struct
        {
            _handlers.TryGetValue(typeof(T), out var existing);
            _handlers[typeof(T)] = (Action<T>)existing + handler;
        }

        public void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            if (_handlers.TryGetValue(typeof(T), out var existing))
                _handlers[typeof(T)] = (Action<T>)existing - handler;
        }
    }
}
