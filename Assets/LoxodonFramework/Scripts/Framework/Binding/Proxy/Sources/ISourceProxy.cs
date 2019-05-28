using System;

namespace Loxodon.Framework.Binding.Proxy.Sources
{
    public interface ISourceProxy : IBindingProxy
    {
        Type Type { get; }

        object Source { get; }
    }
}
