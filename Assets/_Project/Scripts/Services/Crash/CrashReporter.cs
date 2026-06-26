using System;
using UnityEngine;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class CrashReporter : ICrashReporter
    {
        public InitPriority Priority => InitPriority.Critical;
        public bool IsReady { get; private set; }

        public void Initialize()
        {
            Application.logMessageReceived += OnLogMessage;
            IsReady = true;
        }

        public void Dispose()
        {
            Application.logMessageReceived -= OnLogMessage;
            IsReady = false;
        }

        public void Report(Exception exception, string context = null)
        {
            // TODO: forward to analytics / crash backend
            Debug.LogError($"[CrashReporter] {context}: {exception}");
        }

        private void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
                Report(new Exception(condition), stackTrace);
        }
    }
}
