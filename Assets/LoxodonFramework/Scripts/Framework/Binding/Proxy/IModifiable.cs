
namespace Loxodon.Framework.Binding.Proxy
{
    public interface IModifiable
    {
        void SetValue(object value);

        void SetValue<TValue>(TValue value);
    }
}
