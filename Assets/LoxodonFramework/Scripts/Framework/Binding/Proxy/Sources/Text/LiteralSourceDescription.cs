using System;

namespace Loxodon.Framework.Binding.Proxy.Sources.Text
{
    [Serializable]
    public class LiteralSourceDescription : SourceDescription
    {
        public object Literal { get; set; }

        public LiteralSourceDescription()
        {
            this.IsStatic = true;
        }

        public override string ToString()
        {
            return this.Literal == null ? "Literal:null" : "Literal:" + this.Literal.ToString();
        }
    }
}