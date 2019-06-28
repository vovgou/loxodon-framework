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
