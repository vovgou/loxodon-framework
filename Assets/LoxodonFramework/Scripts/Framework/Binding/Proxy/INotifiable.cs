using System;

namespace Loxodon.Framework.Binding.Proxy
{   

    public interface INotifiable<T> where T : EventArgs
    {
        event EventHandler<T> ValueChanged;
    }
}
