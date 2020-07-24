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

using Loxodon.Framework.Contexts;
using Loxodon.Log;

namespace Loxodon.Framework.Views
{
    public class Loading : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Loading));

        private const string DEFAULT_VIEW_LOCATOR_KEY = "_DEFAULT_VIEW_LOCATOR";
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

        private static IUIViewLocator GetUIViewLocator()
        {
            ApplicationContext context = Context.GetApplicationContext();
            IUIViewLocator locator = context.GetService<IUIViewLocator>();
            if (locator == null)
            {
                if (log.IsWarnEnabled)
                    log.Warn("Not found the \"IUIViewLocator\" in the ApplicationContext.Try loading the AlertDialog using the DefaultUIViewLocator.");

                locator = context.GetService<IUIViewLocator>(DEFAULT_VIEW_LOCATOR_KEY);
                if (locator == null)
                {
                    locator = new DefaultUIViewLocator();
                    context.GetContainer().Register(DEFAULT_VIEW_LOCATOR_KEY, locator);
                }
            }
            return locator;
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
                    IUIViewLocator locator = GetUIViewLocator();
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
