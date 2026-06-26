using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface INetworkService : IAsyncService
    {
        bool IsConnected { get; }
    }
}
