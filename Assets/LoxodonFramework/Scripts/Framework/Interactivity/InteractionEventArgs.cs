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
