using System;

namespace Loxodon.Framework.Binding.Converters
{    
    public class GenericConverter<TFrom, TTo> : AbstractConverter<TFrom, TTo>
    {
        private Func<TFrom, TTo> handler;
        private Func<TTo, TFrom> backHandler;

        public GenericConverter(Func<TFrom, TTo> handler, Func<TTo, TFrom> backHandler)
        {
            this.handler = handler;
            this.backHandler = backHandler;
        }

        public override TTo Convert(TFrom value)
        {
            if (this.handler != null)
                return this.handler(value);
            return default(TTo);
        }

        public override TFrom ConvertBack(TTo value)
        {
            if (this.backHandler != null)
                return this.backHandler(value);
            return default(TFrom);
        }
    }
}
