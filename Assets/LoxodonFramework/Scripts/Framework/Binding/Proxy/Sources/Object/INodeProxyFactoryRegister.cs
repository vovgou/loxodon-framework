using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public interface INodeProxyFactoryRegister
    {
        void Register(INodeProxyFactory factory,int priority = 100);

        void Unregister(INodeProxyFactory factory);
    }
}
