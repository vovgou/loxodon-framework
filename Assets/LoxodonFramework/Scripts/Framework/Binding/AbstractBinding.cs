using System;

namespace Loxodon.Framework.Binding
{
    public abstract class AbstractBinding : IBinding
    {
        private readonly object target;
        private object dataContext;

        public AbstractBinding(object dataContext, object target)
        {
            this.target = target;
            this.dataContext = dataContext;
        }

        public object Target
        {
            get { return this.target; }
        }

        public virtual object DataContext
        {
            get { return this.dataContext; }
            set
            {
                if (this.dataContext == value)
                    return;

                this.dataContext = value;
                this.OnDataContextChanged();
            }
        }

        protected abstract void OnDataContextChanged();

        #region IDisposable Support     

        protected virtual void Dispose(bool disposing)
        {
        }

        ~AbstractBinding()
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
