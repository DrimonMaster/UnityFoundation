using System.Threading.Tasks;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class NetworkService : INetworkService
    {
        public InitPriority Priority => InitPriority.Important;
        public bool IsReady { get; private set; }
        public bool IsConnected { get; private set; }

        public void Initialize() { }

        public async Task StartAsync()
        {
            // TODO: connect to backend and verify reachability
            await Task.CompletedTask;
            IsConnected = true;
            IsReady = true;
        }

        public void Dispose()
        {
            IsConnected = false;
            IsReady = false;
        }
    }
}
