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

#if NET_4_6 || NET_STANDARD_2_0
using Loxodon.Framework.Execution;
using Loxodon.Log;
using System;
using UnityEngine;

namespace Loxodon.Framework.Net
{
    public class Progress<T> : IProgress<T>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Progress<T>));

        private readonly bool runOnMainThread;
        private readonly Action<T> handler;

        public Progress() : this(null, true)
        {
        }

        public Progress(Action<T> handler) : this(handler, true)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");
        }

        public Progress(Action<T> handler, bool runOnMainThread)
        {
            this.handler = handler;
            this.runOnMainThread = runOnMainThread;
        }

        public event EventHandler<T> ProgressChanged;

        protected virtual void OnReport(T value)
        {
            try
            {
                Action<T> handler = this.handler;
                EventHandler<T> changedEvent = ProgressChanged;
                if (handler != null || changedEvent != null)
                {
                    if (runOnMainThread)
                        Executors.RunOnMainThread(() => { RaiseProgressChanged(value); });
                    else
                        RaiseProgressChanged(value);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error(e);
            }
        }

        void IProgress<T>.Report(T value)
        {
            OnReport(value);
        }

        private void RaiseProgressChanged(T value)
        {
            Action<T> handler = this.handler;
            EventHandler<T> progressChanged = this.ProgressChanged;

            handler?.Invoke(value);
            progressChanged?.Invoke(this, value);
        }
    }
}
#endif
