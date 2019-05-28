using Loxodon.Framework.Binding.Paths;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public interface INodeProxyFactory
    {
        ISourceProxy Create(object source, PathToken token);
    }
}
