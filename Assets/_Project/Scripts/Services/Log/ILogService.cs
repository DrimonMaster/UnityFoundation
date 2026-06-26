using System;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface ILogService : IService
    {
        void Log(string message, LogCategory category = LogCategory.Core);
        void LogWarning(string message, LogCategory category = LogCategory.Core);
        void LogError(string message, LogCategory category = LogCategory.Core);
        void LogException(Exception e, LogCategory category = LogCategory.Core);
    }
}
