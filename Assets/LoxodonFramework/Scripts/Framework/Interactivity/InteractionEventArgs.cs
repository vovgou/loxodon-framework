using System;

namespace Loxodon.Framework.Interactivity
{
    /// <summary>
    /// Event args for the <see cref="IInteractionRequest.Raised"/> event.
    /// </summary>
    public class InteractionEventArgs : EventArgs
    {
        private object context;

        private Action callback;

        /// <summary>
        /// Constructs a new instance of <see cref="InteractionEventArgs"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="callback"></param>
        public InteractionEventArgs(object context, Action callback)
        {
            this.context = context;
            this.callback = callback;
        }

        /// <summary>
        /// Gets the context for a requested interaction.
        /// </summary>
        public object Context { get { return this.context; } }

        /// <summary>
        /// Gets the callback to execute when an interaction is completed.
        /// </summary>
        public Action Callback { get { return this.callback; } }
    }
}
