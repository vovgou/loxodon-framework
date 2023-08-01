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

using Loxodon.Framework.Binding.Paths;
using Loxodon.Framework.Binding.Proxy.Sources.Object;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using System;

namespace Loxodon.Framework.Binding.Proxy.Sources.Weaving
{
    public class WovenNodeProxyFactory : INodeProxyFactory
    {
        public ISourceProxy Create(object source, PathToken token)
        {
            IPathNode node = token.Current;
            if (source == null || node is IndexedNode)
                return null;

            if (source is IWovenNodeProxyFinder finder)
                return CreateProxy(finder, node);
            return null;
        }

        protected virtual ISourceProxy CreateProxy(IWovenNodeProxyFinder source, IPathNode node)
        {
            var memberNode = node as MemberNode;
            if (memberNode == null)
                return null;

            Type sourceType = source.GetType();
            var memberInfo = memberNode.MemberInfo;
            if (memberInfo != null && !memberInfo.DeclaringType.IsAssignableFrom(sourceType))
                return null;

            ISourceProxy proxy = source.GetSourceProxy(memberNode.Name);
            if (proxy == null)
                return null;

            var valueType = proxy.Type;
            if (typeof(IObservableProperty).IsAssignableFrom(valueType))
            {
                object observableValue = (proxy as IObtainable)?.GetValue();
                if (observableValue == null)
                    return null;

                return new ObservableNodeProxy(source, (IObservableProperty)observableValue);
            }
            else if (typeof(IInteractionRequest).IsAssignableFrom(valueType))
            {
                object request = (proxy as IObtainable)?.GetValue();
                if (request == null)
                    return null;

                return new InteractionNodeProxy(source, (IInteractionRequest)request);
            }
            return proxy;
        }
    }
}
