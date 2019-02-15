namespace Loxodon.Framework.Binding.Reflection
{
    public interface IProxyInvoker
    {
        IProxyMethodInfo ProxyMethodInfo { get; }

        object Invoke(params object[] args);
    }
}
