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
using Loxodon.Framework.Binding.Reflection;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using System;
using System.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class UniversalTargetProxyFactory : ITargetProxyFactory
    {
        private IPathParser pathParser;
        public UniversalTargetProxyFactory(IPathParser pathParser)
        {
            this.pathParser = pathParser;
        }

        public ITargetProxy CreateProxy(object target, BindingDescription description)
        {
            IProxyType type = description.TargetType != null ? description.TargetType.AsProxy() : target.GetType().AsProxy();
            if (TargetNameUtil.IsCollection(description.TargetName))
                return CreateItemProxy(target, type, description);

            IProxyMemberInfo memberInfo = type.GetMember(description.TargetName);
            if (memberInfo == null)
                memberInfo = type.GetMember(description.TargetName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (memberInfo == null)
                throw new MissingMemberException(type.Type.FullName, description.TargetName);

            var propertyInfo = memberInfo as IProxyPropertyInfo;
            if (propertyInfo != null)
            {
                var valueType = propertyInfo.ValueType;
                if (typeof(IObservableProperty).IsAssignableFrom(valueType))
                {
                    object observableValue = propertyInfo.GetValue(target);
                    if (observableValue == null)
                        throw new NullReferenceException(string.Format("The \"{0}\" property is null in class \"{1}\".", propertyInfo.Name, propertyInfo.DeclaringType.Name));

                    return new ObservableTargetProxy(target, (IObservableProperty)observableValue);
                }

                if (typeof(IInteractionAction).IsAssignableFrom(valueType))
                {
                    object interactionAction = propertyInfo.GetValue(target);
                    if (interactionAction == null)
                        return null;

                    return new InteractionTargetProxy(target, (IInteractionAction)interactionAction);
                }

                return new PropertyTargetProxy(target, propertyInfo);
            }

            var fieldInfo = memberInfo as IProxyFieldInfo;
            if (fieldInfo != null)
            {
                var valueType = fieldInfo.ValueType;
                if (typeof(IObservableProperty).IsAssignableFrom(valueType))
                {
                    object observableValue = fieldInfo.GetValue(target);
                    if (observableValue == null)
                        throw new NullReferenceException(string.Format("The \"{0}\" field is null in class \"{1}\".", fieldInfo.Name, fieldInfo.DeclaringType.Name));

                    return new ObservableTargetProxy(target, (IObservableProperty)observableValue);
                }

                if (typeof(IInteractionAction).IsAssignableFrom(valueType))
                {
                    object interactionAction = fieldInfo.GetValue(target);
                    if (interactionAction == null)
                        return null;

                    return new InteractionTargetProxy(target, (IInteractionAction)interactionAction);
                }

                return new FieldTargetProxy(target, fieldInfo);
            }

            var eventInfo = memberInfo as IProxyEventInfo;
            if (eventInfo != null)
                return new EventTargetProxy(target, eventInfo);

            var methodInfo = memberInfo as IProxyMethodInfo;
            if (methodInfo != null)
                return new MethodTargetProxy(target, methodInfo);

            return null;
        }

        private ITargetProxy CreateItemProxy(object target, IProxyType type, BindingDescription description)
        {
            Path path = pathParser.Parse(description.TargetName);
            if (path.Count < 1 || path.Count > 2)
                return null;

            IndexedNode indexNode = null;
            object collectionTarget = null;
            if (path.Count == 1)
            {
                indexNode = (IndexedNode)path[0];
                collectionTarget = target;
            }
            if (path.Count == 2)
            {
                indexNode = (IndexedNode)path[1];
                MemberNode memberNode = (MemberNode)path[0];
                collectionTarget = GetCollectionTarget(type, target, memberNode.Name);
                if (collectionTarget == null)
                    throw new NullReferenceException(string.Format("Unable to bind the \"{0}\". The value of the Property or Field named \"{1}\" cannot be null.", description, memberNode.Name));
            }

            IProxyType proxyType = collectionTarget.GetType().AsProxy();
            IProxyItemInfo itemInfo = proxyType.GetItem();
            if (itemInfo == null)
                throw new MissingMemberException(proxyType.Type.FullName, "Item");

            if (indexNode is IntegerIndexedNode intNode)
            {
                return new ItemTargetProxy<int>(collectionTarget, intNode.Value, itemInfo);
            }
            else if (indexNode is StringIndexedNode stringNode)
            {
                return new ItemTargetProxy<string>(collectionTarget, stringNode.Value, itemInfo);
            }
            return null;
        }

        private static object GetCollectionTarget(IProxyType type, object target, string name)
        {
            IProxyPropertyInfo propertyInfo = type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (propertyInfo != null)
                return propertyInfo.GetValue(target);

            IProxyFieldInfo fieldInfo = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (fieldInfo != null)
                return fieldInfo.GetValue(target);

            throw new MissingMemberException(type.Type.FullName, name);
        }
    }
}
