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

namespace Loxodon.Framework.Utilities
{
    public class TransformEnumerator : IEnumerator
    {
        private IEnumerator enumerator;
        private Converter<object, object> converter;
        public TransformEnumerator(IEnumerator enumerator, Converter<object, object> converter)
        {
            this.enumerator = enumerator;
            this.converter = converter;
        }

        public object Current { get; private set; }

        public bool MoveNext()
        {
            if (this.enumerator.MoveNext())
            {
                var current = this.enumerator.Current;
                this.Current = converter(current);
                return true;
            }
            return false;
        }

        public void Reset()
        {
            this.enumerator.Reset();
        }
    }

    public class TransformEnumerator<TInput, TOutput> : IEnumerator<TOutput>
    {
        private IEnumerator<TInput> enumerator;
        private Converter<TInput, TOutput> converter;
        public TransformEnumerator(IEnumerator<TInput> enumerator, Converter<TInput, TOutput> converter)
        {
            this.enumerator = enumerator;
            this.converter = converter;
        }

        public TOutput Current { get; private set; }

        object IEnumerator.Current { get { return this.Current; } }

        public bool MoveNext()
        {
            if (this.enumerator.MoveNext())
            {
                var current = this.enumerator.Current;
                this.Current = converter(current);
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
                this.converter = null;
                this.disposedValue = true;
            }
        }

        ~TransformEnumerator()
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
