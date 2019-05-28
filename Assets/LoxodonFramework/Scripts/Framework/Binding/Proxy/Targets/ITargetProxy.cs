using System;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public interface ITargetProxy : IBindingProxy
    {
        Type Type { get; }

        object Target { get; }

        BindingMode DefaultMode { get; }
    }
}
