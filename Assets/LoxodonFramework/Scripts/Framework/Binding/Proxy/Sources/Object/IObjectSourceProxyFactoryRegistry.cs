using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public interface IObjectSourceProxyFactoryRegistry
    {
        void Register(ISpecificTypeObjectSourceProxyFactory factory, int priority = 100);

        void Unregister(ISpecificTypeObjectSourceProxyFactory factory);
    }
}
