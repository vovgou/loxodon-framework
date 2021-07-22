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

using Loxodon.Log;
using System;
using UnityEngine;
using XLua;

namespace Loxodon.Framework.Binding.Proxy
{
    public class LuaInvoker : IScriptInvoker, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LuaInvoker));

        private readonly WeakReference target;
        private LuaFunction function;
        public LuaInvoker(object target, LuaFunction function)
        {
            if (target == null)
                throw new ArgumentNullException("target", "Unable to bind to target as it's null");

            this.target = new WeakReference(target, false);
            this.function = function;
        }

        public object Target { get { return this.target != null && this.target.IsAlive ? this.target.Target : null; } }

        public object Invoke(params object[] args)
        {
            try
            {
                var target = this.Target;
                if (target == null)
                    return null;

                if (target is Behaviour behaviour && !behaviour.isActiveAndEnabled)
                    return null;

                int length = args != null ? args.Length + 1 : 1;
                object[] parameters = new object[length];
                parameters[0] = target;
                if (args != null && args.Length > 0)
                    Array.Copy(args, 0, parameters, 1, args.Length);

                return this.function.Call(parameters);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
            return null;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (function != null)
                    {
                        function.Dispose();
                        function = null;
                    }
                }
                disposedValue = true;
            }
        }

        ~LuaInvoker()
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