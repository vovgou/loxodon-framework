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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loxodon.Framework.Utilities
{
    public class FilterEnumerator : IEnumerator
    {
        private IEnumerator enumerator;
        private Predicate<object> match;
        public FilterEnumerator(IEnumerator enumerator, Predicate<object> match)
        {
            this.enumerator = enumerator;
            this.match = match;
        }

        public object Current { get; private set; }

        public bool MoveNext()
        {
            while (this.enumerator.MoveNext())
            {
                var current = this.enumerator.Current;
                if (!match(current))
                    continue;

                this.Current = current;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            this.enumerator.Reset();
        }
    }

    public class FilterEnumerator<T> : IEnumerator<T>
    {
        private IEnumerator<T> enumerator;
        private Predicate<T> match;
        public FilterEnumerator(IEnumerator<T> enumerator, Predicate<T> match)
        {
            this.Current = default(T);
            this.enumerator = enumerator;
            this.match = match;
        }

        public T Current { get; private set; }

        object IEnumerator.Current { get { return this.Current; } }

        public bool MoveNext()
        {
            while (this.enumerator.MoveNext())
            {
                var current = this.enumerator.Current;
                if (!match(current))
                    continue;

                this.Current = current;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            this.enumerator.Reset();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                this.Reset();
                this.enumerator = null;
                this.match = null;
                this.disposedValue = true;
            }
        }

        ~FilterEnumerator()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
