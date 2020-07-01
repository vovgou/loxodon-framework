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

using System.Threading;

namespace Loxodon.Framework.Execution
{
    /// <summary>
    /// 
    /// </summary>
    public class CountFinishedEvent
    {
        private readonly object _lock = new object();
        private readonly ManualResetEvent resetEvent = new ManualResetEvent(false);
        private int count = 0;

        public CountFinishedEvent(int count)
        {
            this.count = count;
        }

        public bool Reset()
        {
            lock (_lock)
            {
                return this.resetEvent.Reset();
            }
        }

        public bool Set()
        {
            lock (_lock)
            {
                if (--count <= 0)
                    return resetEvent.Set();
                return false;
            }
        }

        public bool Wait()
        {
            lock (_lock)
            {
                return resetEvent.WaitOne();
            }
        }

        public bool Wait(int millisecondsTimeout)
        {
            lock (_lock)
            {
                return resetEvent.WaitOne(millisecondsTimeout);
            }
        }
    }
}
