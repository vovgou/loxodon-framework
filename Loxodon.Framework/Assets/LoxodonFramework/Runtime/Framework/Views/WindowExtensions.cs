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
using Loxodon.Framework.Asynchronous;
using System;

namespace Loxodon.Framework.Views
{
    public static class WindowExtensions
    {
        /// <summary>
        /// wait until the window is dismissed
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static Asynchronous.IAsyncResult WaitDismissed(this Window window)
        {
            AsyncResult result = new AsyncResult();
            EventHandler handler = null;
            handler = (sender, eventArgs) =>
            {
                window.OnDismissed -= handler;
                result.SetResult(null);
            };
            window.OnDismissed += handler;
            return result;
        }

        /// <summary>
        /// wait until the view is disabled.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static Asynchronous.IAsyncResult WaitDisabled(this UIView view)
        {
            AsyncResult result = new AsyncResult();
            EventHandler handler = null;
            handler = (sender, eventArgs) =>
            {
                view.OnDisabled -= handler;
                result.SetResult(null);
            };
            view.OnDisabled += handler;
            return result;
        }
    }
}
