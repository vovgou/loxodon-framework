using System;
using System.Reflection;

using Loxodon.Log;
using Loxodon.Framework.Binding.Paths;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Binding.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public class UniversalObjectSourceProxyFactory : ISpecificTypeObjectSourceProxyFactory
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UniversalObjectSourceProxyFactory));

        public bool TryCreateProxy(object source, PathToken token, IObjectSourceProxyFactory factory, out IObjectSourceProxy proxy)
        {
            proxy = null;
            IPathNode node = token.Current;
            if (node is TypeNode)
            {
                TypeNode typeNode = (node as TypeNode);
                Type type = typeNode.Type;
                if (type == null)
                    type = TypeFinderUtils.FindType(typeNode.Name);

                if (type == null || !token.HasNext())
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Unable to bind: not found the \"{0}\" type.", typeNode.Name);

                    return false;
                }

                proxy = CreateStaticProxy(type, token.NextToken(), factory);
                if (proxy != null)
                    return true;
            }
            else
            {
                proxy = CreateProxy(source, token, factory);
                if (proxy != null)
                    return true;
            }

            return false;
        }

        public virtual IObjectSourceProxy CreateStaticProxy(Type type, PathToken token, IObjectSourceProxyFactory factory)
        {
            var node = token.Current;
            var proxy = token.HasNext() ? this.CreateStaticChainedProxy(type, node, token.NextToken(), factory) : this.CreateStaticLeafProxy(type, node);
            if (proxy != null)
                return proxy;

            if (log.IsWarnEnabled)
                log.WarnFormat("Unable to bind: Not found the \"{0}\" member on the \"{1}\" type.", (node as MemberNode).Name, type.Name);

            return null;
        }

        public virtual IObjectSourceProxy CreateProxy(object source, PathToken token, IObjectSourceProxyFactory factory)
        {
            if (source == null)
                return new EmptyObjectSourceProxy();

            var node = token.Current;
            var proxy = token.HasNext() ? this.CreateChainedProxy(source, node, token.NextToken(), factory) : this.CreateLeafProxy(source, node);
            if (proxy != null)
                return proxy;

            if (log.IsWarnEnabled)
                log.WarnFormat("Unable to bind: Not found the \"{0}\" member on the \"{1}\" type.", (node as MemberNode).Name, source.GetType().Name);

            return null;
        }

        protected virtual IObjectSourceProxy CreateChainedProxy(object source, IPathNode node, PathToken nextToken, IObjectSourceProxyFactory factory)
        {
            var indexedNode = node as IndexedNode;
            if (indexedNode != null)
            {
                var itemPropertyInfo = this.FindItemPropertyInfo(source.GetType());
                if (itemPropertyInfo == null)
                    return null;

                return new ChainedItemObjectSourceProxy(source, itemPropertyInfo, nextToken, factory, indexedNode.Value);
            }

            var memberNode = node as MemberNode;
            if (memberNode == null)
                return null;

            var memberInfo = memberNode.MemberInfo;
            if (memberInfo == null)
                memberInfo = source.GetType().FindFirstMemberInfo(memberNode.Name);

            if (memberInfo == null || memberInfo.IsStatic())
                return null;

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                var propertyType = propertyInfo.PropertyType;

                if (typeof(IObservableProperty).IsAssignableFrom(propertyType))
                {
                    IProxyPropertyInfo proxyPropertyInfo = propertyInfo.AsProxy();
                    object observableValue = proxyPropertyInfo.GetValue(source);
                    if (observableValue == null)
                        return null;

                    return new ChainedObservablePropertyObjectSourceProxy(source, (IObservableProperty)observableValue, nextToken, factory);
                }
                else
                {
                    return new ChainedPropertyObjectSourceProxy(source, propertyInfo, nextToken, factory);
                }
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                var fieldType = fieldInfo.FieldType;

                if (typeof(IObservableProperty).IsAssignableFrom(fieldType))
                {
                    IProxyFieldInfo proxyFieldInfo = fieldInfo.AsProxy();
                    object observableValue = proxyFieldInfo.GetValue(source);
                    if (observableValue == null)
                        return null;

                    return new ChainedObservablePropertyObjectSourceProxy(source, (IObservableProperty)observableValue, nextToken, factory);
                }
                else
                {
                    return new ChainedFieldObjectSourceProxy(source, fieldInfo, nextToken, factory);
                }
            }
            return null;
        }

        protected virtual IObjectSourceProxy CreateLeafProxy(object source, IPathNode node)
        {
            var indexedNode = node as IndexedNode;
            if (indexedNode != null)
            {
                var itemPropertyInfo = this.FindItemPropertyInfo(source.GetType());
                if (itemPropertyInfo == null)
                    return null;

                return new LeafItemObjectSourceProxy(source, itemPropertyInfo, indexedNode.Value);
            }

            var memberNode = node as MemberNode;
            if (memberNode == null)
                return null;

            var memberInfo = memberNode.MemberInfo;
            if (memberInfo == null)
                memberInfo = source.GetType().FindFirstMemberInfo(memberNode.Name);

            if (memberInfo == null || memberInfo.IsStatic())
                return null;

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                var propertyType = propertyInfo.PropertyType;

                if (typeof(IObservableProperty).IsAssignableFrom(propertyType))
                {
                    IProxyPropertyInfo proxyPropertyInfo = propertyInfo.AsProxy();
                    object observableValue = proxyPropertyInfo.GetValue(source);
                    if (observableValue == null)
                        return null;

                    return new ObservablePropertyObjectSourceProxy(source, (IObservableProperty)observableValue);
                }
                else
                {
                    return new LeafPropertyObjectSourceProxy(source, propertyInfo);
                }
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                var fieldType = fieldInfo.FieldType;

                if (typeof(IObservableProperty).IsAssignableFrom(fieldType))
                {
                    IProxyFieldInfo proxyFieldInfo = fieldInfo.AsProxy();
                    object observableValue = proxyFieldInfo.GetValue(source);
                    if (observableValue == null)
                        return null;

                    return new ObservablePropertyObjectSourceProxy(source, (IObservableProperty)observableValue);
                }
                else
                {
                    return new LeafFieldObjectSourceProxy(source, fieldInfo);
                }
            }

            var methodInfo = memberInfo as MethodInfo;
            if (methodInfo != null && methodInfo.ReturnType.Equals(typeof(void)))
                return new VoidMethodObjectSourceProxy(source, methodInfo);

            var eventInfo = memberInfo as EventInfo;
            if (eventInfo != null)
                return new EventObjectSourceProxy(source, eventInfo);

            return null;
        }

        protected virtual IObjectSourceProxy CreateStaticChainedProxy(Type type, IPathNode node, PathToken nextToken, IObjectSourceProxyFactory factory)
        {
            var memberNode = node as MemberNode;
            if (memberNode == null)
                return null;

            var memberInfo = memberNode.MemberInfo;
            if (memberInfo == null)
                memberInfo = this.FindStaticMemberInfo(type, memberNode.Name);

            if (memberInfo == null)
                return null;

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                var propertyType = propertyInfo.PropertyType;

                if (typeof(IObservableProperty).IsAssignableFrom(propertyType))
                {
                    IProxyPropertyInfo proxyPropertyInfo = propertyInfo.AsProxy();
                    object observableValue = proxyPropertyInfo.GetValue(null);
                    if (observableValue == null)
                        return null;

                    return new ChainedObservablePropertyObjectSourceProxy((IObservableProperty)observableValue, nextToken, factory);
                }
                else
                {
                    return new ChainedPropertyObjectSourceProxy(propertyInfo, nextToken, factory);
                }
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                var fieldType = fieldInfo.FieldType;

                if (typeof(IObservableProperty).IsAssignableFrom(fieldType))
                {
                    IProxyFieldInfo proxyFieldInfo = fieldInfo.AsProxy();
                    object observableValue = proxyFieldInfo.GetValue(null);
                    if (observableValue == null)
                        return null;

                    return new ChainedObservablePropertyObjectSourceProxy((IObservableProperty)observableValue, nextToken, factory);
                }
                else
                {
                    return new ChainedFieldObjectSourceProxy(fieldInfo, nextToken, factory);
                }
            }
            return null;
        }

        protected virtual IObjectSourceProxy CreateStaticLeafProxy(Type type, IPathNode node)
        {
            var memberNode = node as MemberNode;
            if (memberNode == null)
                return null;

            var memberInfo = memberNode.MemberInfo;
            if (memberInfo == null)
                memberInfo = this.FindStaticMemberInfo(type, memberNode.Name);

            if (memberInfo == null)
                return null;

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                var propertyType = propertyInfo.PropertyType;

                if (typeof(IObservableProperty).IsAssignableFrom(propertyType))
                {
                    IProxyPropertyInfo proxyPropertyInfo = propertyInfo.AsProxy();
                    object observableValue = proxyPropertyInfo.GetValue(null);
                    if (observableValue == null)
                        throw new ArgumentNullException();

                    return new ObservablePropertyObjectSourceProxy((IObservableProperty)observableValue);
                }
                else
                {
                    return new LeafPropertyObjectSourceProxy(null, propertyInfo);
                }
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                var fieldType = fieldInfo.FieldType;

                if (typeof(IObservableProperty).IsAssignableFrom(fieldType))
                {
                    IProxyFieldInfo proxyFieldInfo = fieldInfo.AsProxy();
                    object observableValue = proxyFieldInfo.GetValue(null);
                    if (observableValue == null)
                        throw new ArgumentNullException();

                    return new ObservablePropertyObjectSourceProxy((IObservableProperty)observableValue);
                }
                else
                {
                    return new LeafFieldObjectSourceProxy(fieldInfo);
                }
            }

            var methodInfo = memberInfo as MethodInfo;
            if (methodInfo != null && methodInfo.ReturnType.Equals(typeof(void)))
                return new VoidMethodObjectSourceProxy(methodInfo);

            var eventInfo = memberInfo as EventInfo;
            if (eventInfo != null)
                return new EventObjectSourceProxy(eventInfo);

            return null;
        }

        protected PropertyInfo FindItemPropertyInfo(Type type)
        {
            return type.GetProperty("Item");
        }

        protected MemberInfo FindStaticMemberInfo(Type type, string name)
        {
            return type.FindFirstMemberInfo(name, BindingFlags.Public | BindingFlags.Static);
        }
    }
}
