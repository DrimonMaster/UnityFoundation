using System;
using UnityEngine;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class LogService : ILogService
    {
        private readonly LogSettings _settings;

        public LogService(LogSettings settings) => _settings = settings;

        public InitPriority Priority => InitPriority.Critical;
        public bool IsReady { get; private set; }

        public void Initialize() => IsReady = true;
        public void Dispose() => IsReady = false;

        public void Log(string message, LogCategory category = LogCategory.Core)
        {
            if (!IsEnabled(category)) return;
            Debug.Log($"[LOG] {message}");
        }

        public void LogWarning(string message, LogCategory category = LogCategory.Core)
        {
            if (!IsEnabled(category)) return;
            Debug.LogWarning($"[WARN] {message}");
        }

        public void LogError(string message, LogCategory category = LogCategory.Core)
        {
            if (!IsEnabled(category)) return;
            Debug.LogError($"[ERROR] {message}");
            if (ServiceLocator.TryGet<ICrashReporter>(out var reporter))
                reporter.Report(new Exception(message));
        }

        public void LogException(Exception e, LogCategory category = LogCategory.Core)
        {
            if (!IsEnabled(category)) return;
            Debug.LogException(e);
            if (ServiceLocator.TryGet<ICrashReporter>(out var reporter))
                reporter.Report(e, context: e.TargetSite?.Name);
        }

        // null settings = log everything (before asset is created in Editor)
        private bool IsEnabled(LogCategory category) =>
            _settings == null || _settings.IsEnabled(category);
    }
}
