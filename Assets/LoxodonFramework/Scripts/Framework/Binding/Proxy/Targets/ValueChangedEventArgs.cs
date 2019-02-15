using System;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class ValueChangedEventArgs:EventArgs
    {
        public object Value { get; private set; }

        public ValueChangedEventArgs(object value)
        {
            this.Value = value;
        }
    }
}
