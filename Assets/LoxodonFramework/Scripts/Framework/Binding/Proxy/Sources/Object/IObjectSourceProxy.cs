using System;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public interface IObjectSourceProxy : ISourceProxy, IModifiable,INotifiable<EventArgs>
    {
    }
}
