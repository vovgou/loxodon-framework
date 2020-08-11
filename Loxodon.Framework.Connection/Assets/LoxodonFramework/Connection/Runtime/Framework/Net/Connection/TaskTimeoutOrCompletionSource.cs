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

namespace Loxodon.Framework.Net.Connection
{
    public class TaskTimeoutOrCompletionSource<TResult> : TaskCompletionSource<TResult>
    {
        private long endTime;
        public TaskTimeoutOrCompletionSource(int timeoutMilliseconds)
        {
            this.endTime = DateTime.Now.Ticks + timeoutMilliseconds * TimeSpan.TicksPerMillisecond;
        }

        public TimeSpan Delay { get { return TimeSpan.FromTicks(endTime - DateTime.Now.Ticks); } }

        public bool IsTimeout { get { return (endTime - DateTime.Now.Ticks) <= 0; } }

        public void SetTimeout()
        {
            this.SetException(new TimeoutException("The operation has timed out."));
        }

        public bool TrySetTimeout()
        {
            return this.TrySetException(new TimeoutException("The operation has timed out."));
        }
    }
}
