namespace Loxodon.Framework.Messaging
{
    public class PropertyChangedMessage<T> : MessageBase
    {
        private string propertyName;
        private T oldValue;
        private T newValue;

        public PropertyChangedMessage(T oldValue, T newValue, string propertyName) : this(null, oldValue, newValue, propertyName)
        {
        }

        public PropertyChangedMessage(object sender, T oldValue, T newValue, string propertyName) : base(sender)
        {
            this.propertyName = propertyName;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public string PropertyName { get { return this.propertyName; } }

        public T OldValue { get { return this.oldValue; } }

        public T NewValue { get { return this.newValue; } }
    }
}
