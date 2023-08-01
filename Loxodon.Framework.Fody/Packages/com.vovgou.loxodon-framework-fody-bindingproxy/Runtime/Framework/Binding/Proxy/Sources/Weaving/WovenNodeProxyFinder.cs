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
using System.ComponentModel;
using System.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Sources.Weaving
{
    public class WovenNodeProxyFinder : IWovenNodeProxyFinder
    {
        private ConcurrentDictionary<string, ISourceProxy> proxies;
        private Type type;
        private object source;
        public WovenNodeProxyFinder(object source)
        {
            this.proxies = new ConcurrentDictionary<string, ISourceProxy>();
            this.source = source;
            this.type = source.GetType();
            if (source is INotifyPropertyChanged sourceNotify)
                sourceNotify.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var name = e.PropertyName;
            if (string.IsNullOrEmpty(name))
            {
                foreach (var kv in proxies)
                {
                    if (kv.Value is IPropertyNodeProxy propertyNodeProxy)
                        propertyNodeProxy.OnPropertyChanged();
                }
            }
            else
            {
                ISourceProxy proxy;
                if (proxies.TryGetValue(name, out proxy) && proxy is IPropertyNodeProxy propertyNodeProxy)
                    propertyNodeProxy.OnPropertyChanged();
            }
        }

        public ISourceProxy GetSourceProxy(string name)
        {
            lock (proxies)
            {
                ISourceProxy proxy = null;
                if (proxies.TryGetValue(name, out proxy))
                    return proxy;

                proxy = CreateSourceProxy(name);
                if (proxy != null)
                    proxies.TryAdd(name, proxy);
                return proxy;
            }
        }

        private ISourceProxy CreateSourceProxy(string name)
        {
            var memberInfo = type.FindFirstMemberInfo(name);
            if (memberInfo == null)
                return null;

            Type declaringType = memberInfo.DeclaringType;
            Type memberProxyType = null;
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            if (memberInfo is PropertyInfo)
                memberProxyType = declaringType.GetNestedType(name + "PropertyNodeProxy", flags);
            else if (memberInfo is FieldInfo)
                memberProxyType = declaringType.GetNestedType(name + "FieldNodeProxy", flags);
            else if (memberInfo is MethodInfo)
                memberProxyType = declaringType.GetNestedType(name + "MethodNodeProxy", flags);

            if (memberProxyType == null)
                return null;

            var finalProxyType = declaringType.IsGenericType ? memberProxyType.MakeGenericType(declaringType.GetGenericArguments()) : memberProxyType;
            return (ISourceProxy)Activator.CreateInstance(finalProxyType, source);
        }
    }
}
