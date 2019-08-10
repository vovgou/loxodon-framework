/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;

namespace Loxodon.Framework.Interactivity
{
    /// <summary>
    /// Implementation of the <see cref="IInteractionRequest"/> interface.
    /// </summary>
    public class InteractionRequest : IInteractionRequest
    {
        private static readonly InteractionEventArgs emptyEventArgs = new InteractionEventArgs(null, null);

        private object sender;

        public InteractionRequest() : this(null)
        {
        }

        public InteractionRequest(object sender)
        {
            this.sender = sender != null ? sender : this;
        }

        /// <summary>
        /// Fired when interaction is needed.
        /// </summary>
        public event EventHandler<InteractionEventArgs> Raised;

        /// <summary>
        /// Fires the Raised event.
        /// </summary>
        public void Raise()
        {
            this.Raise(null);
        }

        /// <summary>
        /// Fires the Raised event.
        /// </summary>
        /// <param name="callback">The callback to execute when the interaction is completed.</param>
        public void Raise(Action callback)
        {
            var handler = this.Raised;
            if (handler != null)
                handler(this.sender, callback == null ? emptyEventArgs : new InteractionEventArgs(null, () => { if (callback != null) callback(); }));
        }
    }

    /// <summary>
    /// Implementation of the <see cref="IInteractionRequest"/> interface.
    /// </summary>
    public class InteractionRequest<T> : IInteractionRequest
    {
        private static readonly InteractionEventArgs emptyEventArgs = new InteractionEventArgs(null, null);

        private object sender;
        public InteractionRequest() : this(null)
        {
        }

        public InteractionRequest(object sender)
        {
            this.sender = sender != null ? sender : this;
        }

        /// <summary>
        /// Fired when interaction is needed.
        /// </summary>
        public event EventHandler<InteractionEventArgs> Raised;

        /// <summary>
        /// Fires the Raised event.
        /// </summary>
        /// <param name="context">The context for the interaction request.</param>
        public void Raise(T context)
        {
            this.Raise(context, null);
        }

        /// <summary>
        /// Fires the Raised event.
        /// </summary>
        /// <param name="context">The context for the interaction request.</param>
        /// <param name="callback">The callback to execute when the interaction is completed.</param>
        public void Raise(T context, Action<T> callback)
        {
            var handler = this.Raised;
            if (handler != null)
                handler(this.sender, (context == null && callback == null) ? emptyEventArgs : new InteractionEventArgs(context, () => { if (callback != null) callback(context); }));
        }
    }
}
