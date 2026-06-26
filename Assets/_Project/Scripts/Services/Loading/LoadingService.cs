using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class LoadingService : ILoadingService
    {
        public InitPriority Priority => InitPriority.Important;
        public bool IsReady { get; private set; }
        public bool IsVisible { get; private set; }

        public void Initialize() => IsReady = true;
        public void Dispose() => IsReady = false;

        public void Show() => IsVisible = true;  // TODO: animate in loading screen
        public void Hide() => IsVisible = false; // TODO: animate out loading screen
    }
}
