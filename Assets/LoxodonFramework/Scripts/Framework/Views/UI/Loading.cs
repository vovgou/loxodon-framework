using System;

using Loxodon.Framework.Contexts;

namespace Loxodon.Framework.Views
{
    public class Loading : IDisposable
    {
        private const string DEFAULT_VIEW_NAME = "UI/Loading";
        private static object _lock = new object();
        private static int refCount = 0;
        private static LoadingWindow window;
        private static string viewName;
        private bool ignoreAnimation;
        public static string ViewName
        {
            get { return string.IsNullOrEmpty(viewName) ? DEFAULT_VIEW_NAME : viewName; }
            set { viewName = value; }
        }

        public static Loading Show(bool ignoreAnimation = false)
        {
            return new Loading(ignoreAnimation);
        }

        protected Loading(bool ignoreAnimation)
        {
            this.ignoreAnimation = ignoreAnimation;
            lock (_lock)
            {
                if (refCount <= 0)
                {
                    ApplicationContext context = Context.GetApplicationContext();
                    IUIViewLocator locator = context.GetService<IUIViewLocator>();
                    window = locator.LoadWindow<LoadingWindow>(ViewName);
                    window.Create();
                    window.Show(this.ignoreAnimation);
                }
                refCount++;
            }
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                Execution.Executors.RunOnMainThread(() =>
                {
                    lock (_lock)
                    {
                        refCount--;
                        if (refCount <= 0)
                        {
                            window.Dismiss(this.ignoreAnimation);
                            window = null;
                        }
                    }
                });
            }
        }

        ~Loading()
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
