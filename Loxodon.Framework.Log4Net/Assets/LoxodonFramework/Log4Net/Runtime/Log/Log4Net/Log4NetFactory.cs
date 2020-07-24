using System;

namespace Loxodon.Log.Log4Net
{
    public class Log4NetFactory : ILogFactory
    {
        public ILog GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public ILog GetLogger(Type type)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(type);
            return new Log4NetLogImpl(log);
        }

        public ILog GetLogger(string name)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(name);
            return new Log4NetLogImpl(log);
        }
    }
}
