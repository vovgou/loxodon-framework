namespace Loxodon.Framework.Binding.Proxy
{
    /// <summary>
    /// Supports Lua Function.
    /// </summary>
    public interface IScriptInvoker
    {
        object Invoke(params object[] args);
    }
}
