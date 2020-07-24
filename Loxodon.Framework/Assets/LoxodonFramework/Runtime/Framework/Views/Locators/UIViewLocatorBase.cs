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

namespace Loxodon.Framework.Views
{
    public abstract class UIViewLocatorBase : IUIViewLocator
    {
        public virtual IView LoadView(string name)
        {
            return LoadView<IView>(name);
        }

        public abstract T LoadView<T>(string name) where T : IView;

        public virtual IWindow LoadWindow(string name)
        {
            return LoadWindow<Window>(name);
        }

        public abstract T LoadWindow<T>(string name) where T : IWindow;

        public virtual IWindow LoadWindow(IWindowManager windowManager, string name)
        {
            return LoadWindow<IWindow>(windowManager, name);
        }

        public abstract T LoadWindow<T>(IWindowManager windowManager, string name) where T : IWindow;


        public virtual IProgressResult<float, IView> LoadViewAsync(string name)
        {
            return LoadViewAsync<IView>(name);
        }

        public abstract IProgressResult<float, T> LoadViewAsync<T>(string name) where T : IView;

        public virtual IProgressResult<float, IWindow> LoadWindowAsync(string name)
        {
            return LoadWindowAsync<IWindow>(name);
        }

        public abstract IProgressResult<float, T> LoadWindowAsync<T>(string name) where T : IWindow;

        public virtual IProgressResult<float, IWindow> LoadWindowAsync(IWindowManager windowManager, string name)
        {
            return LoadWindowAsync<IWindow>(windowManager, name);
        }

        public abstract IProgressResult<float, T> LoadWindowAsync<T>(IWindowManager windowManager, string name) where T : IWindow;
    }
}
