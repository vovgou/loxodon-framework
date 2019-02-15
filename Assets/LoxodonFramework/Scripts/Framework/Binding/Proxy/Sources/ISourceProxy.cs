using System;

namespace Loxodon.Framework.Binding.Proxy.Sources
{

    public interface ISourceProxy : IProxy, IObtainable
    {

        Type Type { get; }

        object Source { get; }

    }

}
