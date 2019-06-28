using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
