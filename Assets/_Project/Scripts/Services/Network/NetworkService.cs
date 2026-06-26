using System.Threading.Tasks;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class NetworkService : INetworkService
    {
        public InitPriority Priority => InitPriority.Important;
        public bool IsReady { get; private set; }
        public bool IsConnected { get; private set; }

        public void Initialize()
        {
            "[Lifecycle] NetworkService initialized".Log(LogCategory.Lifecycle);
        }

        public async Task StartAsync()
        {
            "[Lifecycle] NetworkService connecting...".Log(LogCategory.Lifecycle);
            // TODO: connect to backend and verify reachability
            await Task.CompletedTask;
            IsConnected = true;
            IsReady = true;
            "[Lifecycle] NetworkService ready".Log(LogCategory.Lifecycle);
        }

        public void Dispose()
        {
            "[Lifecycle] NetworkService disposed".Log(LogCategory.Lifecycle);
            IsConnected = false;
            IsReady = false;
        }
    }
}
