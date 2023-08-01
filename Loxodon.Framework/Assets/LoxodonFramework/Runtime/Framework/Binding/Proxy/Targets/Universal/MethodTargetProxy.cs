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

using Loxodon.Framework.Binding.Reflection;
using System;
using UnityEngine;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class MethodTargetProxy : TargetProxyBase, IObtainable, IProxyInvoker
    {
        protected readonly IProxyMethodInfo methodInfo;
        public MethodTargetProxy(object target, IProxyMethodInfo methodInfo) : base(target)
        {
            this.methodInfo = methodInfo;
            if (!methodInfo.ReturnType.Equals(typeof(void)))
                throw new ArgumentException("methodInfo");
        }

        public override BindingMode DefaultMode { get { return BindingMode.OneWayToSource; } }

        public override Type Type { get { return typeof(IProxyInvoker); } }

        public IProxyMethodInfo ProxyMethodInfo { get { return this.methodInfo; } }

        public object GetValue()
        {
            return this;
        }

        public TValue GetValue<TValue>()
        {
            return (TValue)this.GetValue();
        }

        public object Invoke(params object[] args)
        {
            if (this.methodInfo.IsStatic)
                return this.methodInfo.Invoke(null, args);

            var obj = this.Target;
            if (obj == null)
                return null;

            if (obj is Behaviour behaviour && !behaviour.isActiveAndEnabled)
                return null;

            return this.methodInfo.Invoke(obj, args);
        }
    }
}
