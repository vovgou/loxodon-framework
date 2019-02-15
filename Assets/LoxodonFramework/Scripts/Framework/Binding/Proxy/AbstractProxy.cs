using System;

namespace Loxodon.Framework.Binding.Proxy
{

    public abstract class AbstractProxy : IProxy
    {

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
        }

         ~AbstractProxy()
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
