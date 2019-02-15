using Loxodon.Framework.Binding.Paths;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public interface ISpecificTypeObjectSourceProxyFactory
    {
        bool TryCreateProxy(object source, PathToken token, IObjectSourceProxyFactory factory, out IObjectSourceProxy proxy);
    }
}
