using System;

namespace Loxodon.Framework.Binding.Proxy.Sources.Text
{
    public class LiteralSourceProxy : AbstractProxy, ITextSourceProxy
    {
        private readonly object source;
        public LiteralSourceProxy(object source)
        {
            this.source = source;
        }

        public Type Type { get { return this.source != null ? this.source.GetType() : typeof(object); } }

        public object Source { get { return this.source; } }

        public object GetValue()
        {
            return this.source;
        }
    }
}
