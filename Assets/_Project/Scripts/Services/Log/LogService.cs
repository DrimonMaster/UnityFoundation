using UnityEngine;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class LogService : ILogService
    {
        public InitPriority Priority => InitPriority.Critical;
        public bool IsReady { get; private set; }

        public void Initialize() => IsReady = true;
        public void Dispose() => IsReady = false;

        public void Log(string message) => Debug.Log(message);
        public void LogWarning(string message) => Debug.LogWarning(message);
        public void LogError(string message) => Debug.LogError(message);
    }
}
