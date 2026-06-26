using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface IUIService : IService
    {
        void Push(string screenId);
        void Pop();
        void Replace(string screenId);
    }
}
