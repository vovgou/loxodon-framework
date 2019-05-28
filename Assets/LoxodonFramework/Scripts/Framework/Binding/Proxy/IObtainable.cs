namespace Loxodon.Framework.Binding.Proxy
{
    public interface IObtainable
    {
        object GetValue();

        TValue GetValue<TValue>();
    }
}
