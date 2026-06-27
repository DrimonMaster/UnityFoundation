using System;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface IEventBus : IService
    {
        IDisposable Subscribe<T>(Action<T> handler, int priority = 0);
        void Unsubscribe<T>(Action<T> handler);
        void Publish<T>(T eventData);
        IDisposable SubscribeOnce<T>(Action<T> handler, int priority = 0);
    }
}
