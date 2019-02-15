using System;

namespace Loxodon.Framework.Binding.Proxy.Targets
{

    public interface ITargetProxy : IProxy
    {
        Type Type { get; }

        BindingMode DefaultMode { get; }

        object Target { get; }

    }
}
