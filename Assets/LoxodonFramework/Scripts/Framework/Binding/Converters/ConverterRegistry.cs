using Loxodon.Framework.Binding.Registry;

namespace Loxodon.Framework.Binding.Converters
{
    public class ConverterRegistry: KeyValueRegistry<string,IConverter>, IConverterRegistry
    {
        public ConverterRegistry()
        {
            this.Init();
        }

        protected virtual void Init()
        {
        }
    }
}
