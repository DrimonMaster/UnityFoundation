using System;
using UnityEngine;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class LogService : ILogService
    {
        private readonly LogSettings _settings;
        private readonly ICrashReporter _crashReporter;

        public LogService()
        {
            _settings = Resources.Load<LogSettings>("LogSettings");
            _crashReporter = ServiceLocator.Get<ICrashReporter>();
        }

        public InitPriority Priority => InitPriority.Critical;
        public bool IsReady { get; private set; }

        public void Initialize()
        {
            IsReady = true;
            "[Lifecycle] LogService initialized".Log(LogCategory.Lifecycle);
        }

        public void Dispose()
        {
            "[Lifecycle] LogService disposed".Log(LogCategory.Lifecycle);
            IsReady = false;
        }

        public void Log(string message, LogCategory category = LogCategory.Core)
        {
            if (!IsEnabled(category)) return;
            Debug.Log(Format(message, category));
        }

        public void LogWarning(string message, LogCategory category = LogCategory.Core)
        {
            if (!IsEnabled(category)) return;
            Debug.LogWarning(Format(message, category));
        }

        public void LogError(string message, LogCategory category = LogCategory.Core)
        {
            if (!IsEnabled(category)) return;
            Debug.LogError(Format(message, category));
            _crashReporter.Report(new Exception(message));
        }

        public void LogException(Exception e, LogCategory category = LogCategory.Core)
        {
            if (!IsEnabled(category)) return;
            _crashReporter.Report(e, e.TargetSite?.Name);
        }

        private bool IsEnabled(LogCategory category) =>
            _settings == null || _settings.IsEnabled(category);

        private string Format(string message, LogCategory category) =>
            _settings != null ? _settings.BuildMessage(message, category) : $"[{category}] {message}";
    }
}
