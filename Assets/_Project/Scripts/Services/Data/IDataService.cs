using System.Threading.Tasks;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface IDataService : IAsyncService
    {
        Task<T> LoadAsync<T>(string key) where T : class, new();
        Task SaveAsync<T>(string key, T data) where T : class;
        bool HasKey(string key);
        void DeleteKey(string key);
    }
}
