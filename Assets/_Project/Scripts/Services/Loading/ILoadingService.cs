using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface ILoadingService : IService
    {
        bool IsVisible { get; }
        void Show();
        void Hide();
    }
}
