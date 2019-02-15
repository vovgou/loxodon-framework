namespace Loxodon.Framework.Binding.Converters
{
    public interface IConverter
    {
        object Convert(object value);

        object ConvertBack(object value);
    }

    public interface IConverter<TFrom, TTo> : IConverter
    {
        TTo Convert(TFrom value);

        TFrom ConvertBack(TTo value);
    }
}
