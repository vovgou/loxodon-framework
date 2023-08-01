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
using System.Collections.Concurrent;
using UnityEngine;

namespace Loxodon.Framework.Binding.Reflection
{
    public class ProxyFactory
    {
        public static readonly ProxyFactory Default = new ProxyFactory();

        private readonly object _lock = new object();
        private readonly ConcurrentDictionary<Type, ProxyType> types = new ConcurrentDictionary<Type, ProxyType>();

        //For compatibility with the "Configurable Enter Play Mode" feature
#if UNITY_2019_3_OR_NEWER //&& UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void OnInitialize()
        {
            Default.types.Clear();
        }
#endif
        internal ConcurrentDictionary<Type, ProxyType> Types { get { return types; } }

        internal virtual ProxyType GetType(Type type, bool create = true)
        {
            ProxyType ret;
            if (this.types.TryGetValue(type, out ret) && ret != null)
                return ret;

            return create ? this.types.GetOrAdd(type, (t) => new ProxyType(t, this)) : null;
        }

        public IProxyType Get(Type type)
        {
            return GetType(type, true);
        }

        public void Register(IProxyMemberInfo proxyMemberInfo)
        {
            if (proxyMemberInfo == null)
                return;

            ProxyType proxyType = this.GetType(proxyMemberInfo.DeclaringType);
            proxyType.Register(proxyMemberInfo);
        }

        public void Unregister(IProxyMemberInfo proxyMemberInfo)
        {
            if (proxyMemberInfo == null)
                return;

            ProxyType proxyType = this.GetType(proxyMemberInfo.DeclaringType);
            proxyType.Unregister(proxyMemberInfo);
        }
    }
}
