using System;

namespace Loxodon.Framework.Binding.Proxy
{

    public interface INotifiable
    {
        event EventHandler ValueChanged;
    }
}
