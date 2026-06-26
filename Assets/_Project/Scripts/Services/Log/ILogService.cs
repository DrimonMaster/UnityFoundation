using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface ILogService : IService
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}
