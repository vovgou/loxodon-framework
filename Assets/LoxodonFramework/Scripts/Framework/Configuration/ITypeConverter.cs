using System;

namespace Loxodon.Framework.Configurations
{
    public interface ITypeConverter
    {
        bool Support(Type type);

        object Convert(Type type, object value);
    }
}
