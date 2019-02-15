using System;

namespace Loxodon.Framework.Binding.Converters
{
    public abstract class AbstractConverter : IConverter
    {
        public virtual object Convert(object value)
        {
            return value;
        }

        public virtual object ConvertBack(object value)
        {
            return value;
        }
    }

    public abstract class AbstractConverter<TFrom, TTo> : IConverter<TFrom, TTo>
    {
        public virtual object Convert(object value)
        {
            return Convert((TFrom)value);
        }

        public virtual TTo Convert(TFrom value)
        {
            throw new NotImplementedException();
        }

        public virtual object ConvertBack(object value)
        {
            return ConvertBack((TTo)value);
        }

        public virtual TFrom ConvertBack(TTo value)
        {
            throw new NotImplementedException();
        }
    }
}