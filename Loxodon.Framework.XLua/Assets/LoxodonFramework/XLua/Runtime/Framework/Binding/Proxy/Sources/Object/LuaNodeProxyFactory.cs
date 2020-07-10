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
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using XLua;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public class LuaNodeProxyFactory : INodeProxyFactory
    {
        public ISourceProxy Create(object source, PathToken token)
        {
            if (source == null || !(source is LuaTable) || token.Path.IsStatic)
                return null;

            IPathNode node = token.Current;
            LuaTable table = (LuaTable)source;

            //if (!Contains(table, node))
            //    return null;

            return CreateProxy(table, node);
        }

        protected virtual bool Contains(LuaTable table, IPathNode node)
        {
            var indexedNode = node as IndexedNode;
            if (indexedNode != null)
                return table.ContainsKey(indexedNode.Value);

            var memberNode = node as MemberNode;
            if (memberNode != null)
                return table.ContainsKey(memberNode.Name);

            return false;
        }

        protected virtual ISourceProxy CreateProxy(LuaTable table, IPathNode node)
        {
            var indexedNode = node as IndexedNode;
            if (indexedNode != null)
            {
                if (indexedNode.Value is int)
                    return new LuaIntTableNodeProxy(table, (int)indexedNode.Value);

                if (indexedNode.Value is string)
                    return new LuaStringTableNodeProxy(table, (string)indexedNode.Value);

                return null;
            }

            var memberNode = node as MemberNode;
            if (memberNode != null)
            {
                var obj = table.Get<object>(memberNode.Name);
                if (obj != null)
                {
                    LuaFunction function = obj as LuaFunction;
                    if (function != null)
                        return new LuaMethodNodeProxy(table, function);

                    IObservableProperty observableValue = obj as IObservableProperty;
                    if (observableValue != null)
                        return new ObservableNodeProxy(table, observableValue);

                    IInteractionRequest request = obj as IInteractionRequest;
                    if (request != null)
                        return new InteractionNodeProxy(table, request);
                }

                return new LuaStringTableNodeProxy(table, memberNode.Name);
            }
            return null;
        }
    }
}
