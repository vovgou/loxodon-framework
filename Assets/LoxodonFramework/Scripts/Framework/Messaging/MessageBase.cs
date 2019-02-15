using System;

namespace Loxodon.Framework.Messaging
{
    public class MessageBase : EventArgs
    {
        private object sender;

        public MessageBase(object sender)
        {
            this.sender = sender;
        }

        /// <summary>
        /// Gets or sets the message's sender.
        /// </summary>
        public object Sender
        {
            get { return this.sender; }
            protected set { this.sender = value; }
        }
    }
}
