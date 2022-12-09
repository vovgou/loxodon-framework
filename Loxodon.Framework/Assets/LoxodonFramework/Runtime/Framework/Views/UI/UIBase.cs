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

using Loxodon.Framework.Contexts;
using Loxodon.Log;

namespace Loxodon.Framework.Views
{
    public abstract class UIBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UIBase));
        private const string DEFAULT_VIEW_LOCATOR_KEY = "_DEFAULT_VIEW_LOCATOR";

        protected static IUIViewLocator GetUIViewLocator()
        {
            ApplicationContext context = Context.GetApplicationContext();
            IUIViewLocator locator = context.GetService<IUIViewLocator>();
            if (locator != null)
                return locator;

            if (log.IsWarnEnabled)
                log.Warn("Not found the \"IUIViewLocator\" in the ApplicationContext.Try loading the Tips using the DefaultUIViewLocator.");

            locator = context.GetService<IUIViewLocator>(DEFAULT_VIEW_LOCATOR_KEY);
            if (locator == null)
            {
                locator = new DefaultUIViewLocator();
                context.GetContainer().Register(DEFAULT_VIEW_LOCATOR_KEY, locator);
            }
            return locator;
        }

        protected static IUIViewGroup GetCurrentViewGroup()
        {
            GlobalWindowManagerBase windowManager = GlobalWindowManagerBase.Root;
            IWindow window = windowManager.Current;
            while (window is WindowContainer windowContainer)
                window = windowContainer.Current;
            return window as IUIViewGroup;
        }
    }
}
