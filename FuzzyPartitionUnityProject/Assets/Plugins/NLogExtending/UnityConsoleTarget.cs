using NLog;
using NLog.Config;
using NLog.Targets;
using UnityEngine;

namespace UnityLogger
{
    [Target("UnityConsole")]
    public sealed class UnityConsoleTarget : TargetWithLayout
    {
        public UnityConsoleTarget()
        {
            Host = "localhost";
        }

        [RequiredParameter]
        public string Host { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            var logMessage = Layout.Render(logEvent);

            if (logEvent.Level == LogLevel.Trace)
                Debug.Log(logMessage);
            if (logEvent.Level == LogLevel.Info)
                Debug.Log(logMessage);
            if (logEvent.Level == LogLevel.Debug)
                Debug.Log(logMessage);
            if (logEvent.Level == LogLevel.Warn)
                Debug.LogWarning(logMessage);
            if (logEvent.Level == LogLevel.Error)
                Debug.LogError(logMessage);
            if (logEvent.Level == LogLevel.Fatal)
                Debug.LogError(logMessage);
        }
    }
}
