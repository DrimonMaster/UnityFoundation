using System;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface ICrashReporter : IService
    {
        void Report(Exception exception, string context = null);
    }
}
