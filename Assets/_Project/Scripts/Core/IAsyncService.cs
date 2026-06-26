using System.Threading.Tasks;

namespace UnityFoundation.Core
{
    public interface IAsyncService : IService
    {
        Task StartAsync();
    }
}
