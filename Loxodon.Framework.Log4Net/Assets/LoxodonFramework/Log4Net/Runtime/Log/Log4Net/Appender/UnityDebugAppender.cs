
namespace Loxodon.Log.Log4Net.Appender
{
    using UnityEngine;
    using log4net.Core;
    using log4net.Appender;

    public class UnityDebugAppender: AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            Level level = loggingEvent.Level;
            if (Level.Fatal.Equals(level) || Level.Error.Equals(level))
                Debug.LogError(RenderLoggingEvent(loggingEvent));
            else if(Level.Warn.Equals(level))
                Debug.LogWarning(RenderLoggingEvent(loggingEvent));
            else
                Debug.Log(RenderLoggingEvent(loggingEvent));
        }
    }
}
