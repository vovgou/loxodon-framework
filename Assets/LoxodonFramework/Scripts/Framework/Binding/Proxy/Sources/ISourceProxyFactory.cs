using Loxodon.Framework.Binding.Paths;

namespace Loxodon.Framework.Binding.Proxy.Sources
{
    public interface ISourceProxyFactory
    {
        ISourceProxy CreateProxy(object source, SourceDescription description);
    }
}
