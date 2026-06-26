using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface IDataService : IAsyncService
    {
        bool TryLoad<T>(string key, out T data) where T : class;
        void Save<T>(string key, T data) where T : class;
    }
}
