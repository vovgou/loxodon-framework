using NLog;
using NLog.Targets;

namespace Loxodon.Log.NLogger.Targets
{
    [Target("UnityConsole")]
    public class UnityConsoleTarget : TargetWithLayout
    {
        protected override void Write(LogEventInfo logEvent)
        {
            LogLevel level = logEvent.Level;
            if (LogLevel.Off.Equals(level))
                return;

            string message = RenderLogEvent(Layout, logEvent);
            if (message == null)
                return;

            if (level.Equals(LogLevel.Trace) || level.Equals(LogLevel.Debug) || level.Equals(LogLevel.Info))
                UnityEngine.Debug.Log(message);
            if (level.Equals(LogLevel.Warn))
                UnityEngine.Debug.LogWarning(message);
            if (level.Equals(LogLevel.Error) || level.Equals(LogLevel.Fatal))
                UnityEngine.Debug.LogError(message);
        }
    }
}