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

namespace Loxodon.Framework.XLua.Loaders
{
    public abstract class PathLoaderBase : LoaderBase, IDisposable
    {
        protected string prefix = "";
        protected string suffix = ".lua.txt";

        public PathLoaderBase(string prefix, string suffix)
        {
            this.prefix = prefix;
            if (!string.IsNullOrEmpty(this.prefix))
                this.prefix = this.prefix.Replace(@"\", "/");
            if (!this.prefix.EndsWith("/"))
                this.prefix += "/";

            this.suffix = suffix;
        }

        protected virtual string GetFullname(string className)
        {
            return string.Format("{0}{1}{2}", prefix, className.Replace(".", "/"), suffix);
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
        }

        ~PathLoaderBase()
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