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
            while (_pending.Count > 0)
            {
                var (e, ctx) = _pending.Dequeue();
                SendToBackend(e, ctx);
            }
        }

        public void Dispose()
        {
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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogException(e);
#endif
            SendToBackend(e, context);
        }

        private static void SendToBackend(Exception e, string context)
        {
            // TODO: Datadog / crash analytics
        }
    }
}
