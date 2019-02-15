using Loxodon.Framework.Binding.Registry;

namespace Loxodon.Framework.Binding.Converters
{
    public interface IConverterRegistry : IKeyValueRegistry<string, IConverter>
    {
    }
}
