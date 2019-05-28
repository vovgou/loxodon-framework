using System;

namespace Loxodon.Framework.Binding.Proxy.Sources.Text
{
    public class LiteralSourceProxy : SourceProxyBase, ISourceProxy, IObtainable
    {
        public LiteralSourceProxy(object source) : base(source)
        {
        }

        public override Type Type { get { return this.source != null ? this.source.GetType() : typeof(object); } }

        public virtual object GetValue()
        {
            return this.source;
        }

        public virtual TValue GetValue<TValue>()
        {
            return (TValue)Convert.ChangeType(this.source, typeof(TValue));
        }
    }
}
