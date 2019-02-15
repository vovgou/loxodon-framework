using Loxodon.Framework.Binding.Paths;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public interface IObjectSourceProxyFactory
    {
        IObjectSourceProxy CreateProxy(object source, PathToken token);
    }
}
