using System;
using Loxodon.Log;

namespace Loxodon.Framework.Binding.Proxy.Sources
{
    public class EmptSourceProxy : SourceProxyBase, IObtainable, IModifiable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmptSourceProxy));

        private SourceDescription description;

        public EmptSourceProxy(SourceDescription description) : base(null)
        {
            this.description = description;
        }

        public override Type Type { get { return typeof(object); } }

        public virtual object GetValue()
        {
            if (log.IsWarnEnabled)
                log.WarnFormat("this is an empty source proxy,If you see this, then the DataContext/SourceObject is null.The SourceDescription is \"{0}\"", description.ToString());

            return null;
        }

        public virtual TValue GetValue<TValue>()
        {
            if (log.IsWarnEnabled)
                log.WarnFormat("this is an empty source proxy,If you see this, then the DataContext/SourceObject is null.The SourceDescription is \"{0}\"", description.ToString());

            return default(TValue);
        }

        public virtual void SetValue(object value)
        {
            if (log.IsWarnEnabled)
                log.WarnFormat("this is an empty source proxy,If you see this, then the DataContext/SourceObject is null.The SourceDescription is \"{0}\"", description.ToString());
        }

        public virtual void SetValue<TValue>(TValue value)
        {
            if (log.IsWarnEnabled)
                log.WarnFormat("this is an empty source proxy,If you see this, then the DataContext/SourceObject is null.The SourceDescription is \"{0}\"", description.ToString());
        }
    }
}
