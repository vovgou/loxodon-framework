using System;

namespace Loxodon.Log.Log4Net
{
    public class Log4NetLogImpl : ILog
    {
        private log4net.ILog log;
        public Log4NetLogImpl(log4net.ILog log)
        {
            this.log = log;
        }

        public bool IsDebugEnabled { get { return log.IsDebugEnabled; } }

        public bool IsInfoEnabled { get { return log.IsInfoEnabled; } }

        public bool IsWarnEnabled { get { return log.IsWarnEnabled; } }

        public bool IsErrorEnabled { get { return log.IsErrorEnabled; } }

        public bool IsFatalEnabled { get { return log.IsFatalEnabled; } }

        public void Debug(object message)
        {
            log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            log.Debug(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            log.DebugFormat(format, args);
        }

        public void Error(object message)
        {
            log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            log.Error(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            log.ErrorFormat(format, args);
        }

        public void Fatal(object message)
        {
            log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            log.Fatal(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            log.FatalFormat(format, args);
        }

        public void Info(object message)
        {
            log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            log.Info(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            log.InfoFormat(format, args);
        }

        public void Warn(object message)
        {
            log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            log.Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            log.WarnFormat(format, args);
        }
    }
}
