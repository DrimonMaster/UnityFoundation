using System;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface IEventBus : IService
    {
        IDisposable Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>(Action<T> handler);
        void Publish<T>(T eventData);
    }
}
