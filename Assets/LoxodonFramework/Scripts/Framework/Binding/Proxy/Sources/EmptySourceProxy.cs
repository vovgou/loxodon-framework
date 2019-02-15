using System;
using Loxodon.Log;

namespace Loxodon.Framework.Binding.Proxy.Sources
{
    public class EmptSourceProxy : AbstractProxy, ISourceProxy, IModifiable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmptSourceProxy));

        private SourceDescription description;
        public EmptSourceProxy(SourceDescription description)
        {
            this.description = description;
        }

        public object Source { get { return null; } }

        public Type Type { get { return typeof(object); } }

        public virtual object GetValue()
        {
            if (log.IsWarnEnabled)
                log.WarnFormat("this is an empty source proxy,If you see this, then the DataContext/SourceObject is null.The SourceDescription is \"{0}\"", description.ToString());

            return null;
        }

        public virtual void SetValue(object value)
        {
            if (log.IsWarnEnabled)
                log.WarnFormat("this is an empty source proxy,If you see this, then the DataContext/SourceObject is null.The SourceDescription is \"{0}\"", description.ToString());
        }
    }
}
