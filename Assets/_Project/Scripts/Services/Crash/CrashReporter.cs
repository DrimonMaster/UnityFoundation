using System;
using System.Collections.Generic;
using UnityEngine;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class CrashReporter : ICrashReporter
    {
        private readonly Queue<(Exception e, string context)> _pending = new();

        public InitPriority Priority => InitPriority.Critical;
        public bool IsReady { get; private set; }

        public void Initialize()
        {
            IsReady = true;
            if (_pending.Count > 0)
                $"[Lifecycle] CrashReporter draining {_pending.Count} pending reports".Log(LogCategory.Lifecycle);
            while (_pending.Count > 0)
            {
                var (e, ctx) = _pending.Dequeue();
                SendToBackend(e, ctx);
            }
            "[Lifecycle] CrashReporter initialized".Log(LogCategory.Lifecycle);
        }

        public void Dispose()
        {
            "[Lifecycle] CrashReporter disposed".Log(LogCategory.Lifecycle);
            _pending.Clear();
            IsReady = false;
        }

        public void Report(Exception e, string context = null)
        {
            if (!IsReady)
            {
                _pending.Enqueue((e, context));
                return;
            }
            SendToBackend(e, context);
        }

        private static void SendToBackend(Exception e, string context)
        {
            // TODO: Datadog / crash analytics
        }
    }
}
