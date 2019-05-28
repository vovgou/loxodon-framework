using System;

namespace Loxodon.Framework.Binding.Proxy
{
    public abstract class BindingProxyBase : IBindingProxy
    {
        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
        }

        ~BindingProxyBase()
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
