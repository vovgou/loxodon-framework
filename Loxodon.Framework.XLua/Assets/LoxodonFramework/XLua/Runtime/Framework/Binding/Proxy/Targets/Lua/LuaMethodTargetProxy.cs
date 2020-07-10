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
using XLua;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class LuaMethodTargetProxy : TargetProxyBase, IObtainable
    {
        private bool disposed = false;
        private IScriptInvoker invoker;

        public LuaMethodTargetProxy(object target, LuaFunction function) : base(target)
        {
            if (target == null)
                throw new ArgumentNullException("target", "Unable to bind to target as it's null");

            this.invoker = new LuaInvoker(target, function);
        }

        public override BindingMode DefaultMode { get { return BindingMode.OneWayToSource; } }

        public override Type Type { get { return typeof(LuaFunction); } }

        public object GetValue()
        {
            return this.invoker;
        }

        public TValue GetValue<TValue>()
        {
            return (TValue)this.invoker;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (invoker != null && invoker is IDisposable)
                    {
                        (invoker as IDisposable).Dispose();
                        invoker = null;
                    }
                }
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}