using System;
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

        public void Log(string message)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"[LOG] {message}");
#endif
        }

        public void LogWarning(string message)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogWarning($"[WARN] {message}");
#endif
        }

        public void LogError(string message)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogError($"[ERROR] {message}");
#endif
            if (ServiceLocator.TryGet<ICrashReporter>(out var reporter))
                reporter.Report(new Exception(message));
        }

        public void LogException(Exception e)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogException(e);
#endif
            if (ServiceLocator.TryGet<ICrashReporter>(out var reporter))
                reporter.Report(e, context: e.TargetSite?.Name);
        }
    }
}
