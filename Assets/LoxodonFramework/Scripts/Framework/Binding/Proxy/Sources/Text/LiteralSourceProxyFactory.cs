namespace Loxodon.Framework.Binding.Proxy.Sources.Text
{   
    public class LiteralSourceProxyFactory : TypedSourceProxyFactory<LiteralSourceDescription>
    {
        protected override bool TryCreateProxy(object source, LiteralSourceDescription description, out ISourceProxy proxy)
        {
            proxy = new LiteralSourceProxy(description.Literal);
            return true;
        }
    }
}
