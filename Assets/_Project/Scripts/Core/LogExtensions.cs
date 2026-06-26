using System;
using System.Diagnostics;
using UnityFoundation.Services;

namespace UnityFoundation.Core
{
    public static class LogExtensions
    {
        [Conditional("ENABLE_LOG")]
        public static void Log(this object message, LogCategory category = LogCategory.Core)
        {
            if (ServiceLocator.TryGet<ILogService>(out var log))
                log.Log(message?.ToString(), category);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogWarning(this object message, LogCategory category = LogCategory.Core)
        {
            if (ServiceLocator.TryGet<ILogService>(out var log))
                log.LogWarning(message?.ToString(), category);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogError(this object message, LogCategory category = LogCategory.Core)
        {
            if (ServiceLocator.TryGet<ILogService>(out var log))
                log.LogError(message?.ToString(), category);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogException(this Exception e, LogCategory category = LogCategory.Core)
        {
            if (ServiceLocator.TryGet<ILogService>(out var log))
                log.LogException(e, category);
        }
    }
}
