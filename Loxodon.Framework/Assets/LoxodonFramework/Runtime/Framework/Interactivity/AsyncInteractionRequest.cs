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
using System.Threading.Tasks;

namespace Loxodon.Framework.Interactivity
{
    /// <summary>
    /// Implementation of the <see cref="IInteractionRequest"/> interface.
    /// </summary>
    public class AsyncInteractionRequest : IInteractionRequest
    {
        private object sender;

        public AsyncInteractionRequest() : this(null)
        {
        }

        public AsyncInteractionRequest(object sender)
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
        public Task Raise()
        {
            TaskCompletionSource<object> source = new TaskCompletionSource<object>();
            this.Raised?.Invoke(this.sender, new AsyncInteractionEventArgs(source, null));
            return source.Task;
        }
    }

    /// <summary>
    /// Implementation of the <see cref="IInteractionRequest"/> interface.
    /// </summary>
    public class AsyncInteractionRequest<T> : IInteractionRequest
    {
        private object sender;
        public AsyncInteractionRequest() : this(null)
        {
        }

        public AsyncInteractionRequest(object sender)
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
        public async Task<T> Raise(T context)
        {
            TaskCompletionSource<object> source = new TaskCompletionSource<object>();
            this.Raised?.Invoke(this.sender, new AsyncInteractionEventArgs(source, context));
            await source.Task;
            return context;
        }
    }
}
