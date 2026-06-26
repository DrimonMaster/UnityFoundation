using System;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface IEventBus : IService
    {
        void Publish<T>(T evt) where T : struct;
        void Subscribe<T>(Action<T> handler) where T : struct;
        void Unsubscribe<T>(Action<T> handler) where T : struct;
    }
}
