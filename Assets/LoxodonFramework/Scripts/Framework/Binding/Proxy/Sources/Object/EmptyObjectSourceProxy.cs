using Loxodon.Log;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public class EmptyObjectSourceProxy : AbstractObjectSourceProxy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmptyObjectSourceProxy));

        public EmptyObjectSourceProxy() : this(null)
        {
        }

        public EmptyObjectSourceProxy(object source) : base(source)
        {
        }

        public override object GetValue()
        {
            if (log.IsWarnEnabled)
                log.WarnFormat("this is an empty source proxy,If you see this, then the SourceObject is null.");

            return null;
        }

        public override void SetValue(object value)
        {
            if (log.IsWarnEnabled)
                log.WarnFormat("this is an empty source proxy,If you see this, then the SourceObject is null.");
        }
    }
}
