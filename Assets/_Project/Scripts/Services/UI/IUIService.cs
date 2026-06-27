using UnityEngine;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface IUIService : IService
    {
        void Show<T>() where T : MonoBehaviour, IScreen;
        void Hide<T>() where T : MonoBehaviour, IScreen;
        void Push<T>() where T : MonoBehaviour, IScreen;
        void Pop();
        void Replace<T>() where T : MonoBehaviour, IScreen;
        void HideAll();
    }
}
