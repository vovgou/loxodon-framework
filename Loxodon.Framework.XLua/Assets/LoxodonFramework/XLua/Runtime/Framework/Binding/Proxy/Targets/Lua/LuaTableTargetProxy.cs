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
    public class LuaTableTargetProxy : ValueTargetProxyBase
    {
        protected readonly string key;
        protected readonly LuaTable metatable;
        public LuaTableTargetProxy(object target, string key) : base(target)
        {
            if (target is ILuaExtendable)
                this.metatable = (target as ILuaExtendable).GetMetatable();
            this.key = key;
        }

        public override Type Type { get { return typeof(object); } }

        public override object GetValue()
        {
            return this.metatable.Get<string, object>(this.key);
        }

        public override TValue GetValue<TValue>()
        {
            return this.metatable.Get<string, TValue>(this.key);
        }

        public override void SetValue(object value)
        {
            this.metatable.Set<string, object>(this.key, value);
        }

        public override void SetValue<TValue>(TValue value)
        {
            this.metatable.Set<string, TValue>(this.key, value);
        }
    }
}
