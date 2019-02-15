//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Linq;

//using Loxodon.Log;
//using Loxodon.Framework.Binding.Reflection;
//using Loxodon.Framework.Observables;
//using Loxodon.Framework.Binding.Paths;

//namespace Loxodon.Framework.Binding.Proxy.Sources.Object
//{
//    public class ObjectSourceProxyFactory : TypedSourceProxyFactory<ObjectSourceDescription>, IObjectSourceProxyFactory
//    {
//        private static readonly ILog log = LogManager.GetLogger(typeof(ObjectSourceProxyFactory));

//        protected override bool TryCreateProxy(object source, ObjectSourceDescription description, out ISourceProxy proxy)
//        {
//            proxy = null;
//            List<IPathNode> nodes = description.Path.ToList();
//            if (description.Path.IsStatic)
//            {
//                if (nodes.Count < 2)
//                {
//                    if (log.IsWarnEnabled)
//                        log.WarnFormat("Unable to bind: the \"{0}\" path be unsupported.", description.Path.ToString());

//                    return false;
//                }

//                TypeNode typeNode = (nodes[0] as TypeNode);
//                Type type = typeNode.Type;
//                if (type == null)
//                    type = TypeFinderUtils.FindType(typeNode.Name);

//                if (type == null)
//                {
//                    if (log.IsWarnEnabled)
//                        log.WarnFormat("Unable to bind: not found the \"{0}\" type.", typeNode.Name);

//                    return false;
//                }

//                var remainingNodes = nodes.Skip(1).ToList();
//                proxy = CreateStaticProxy(type, remainingNodes);
//                if (proxy != null)
//                    return true;
//            }
//            else
//            {
//                if (nodes.Count < 1)
//                {
//                    if (log.IsWarnEnabled)
//                        log.Warn("Unable to bind: an empty path node list!.");

//                    return false;
//                }

//                proxy = CreateProxy(source, nodes);
//                if (proxy != null)
//                    return true;
//            }

//            return false;
//        }

//        public virtual IObjectSourceProxy CreateStaticProxy(Type type, List<IPathNode> nodes)
//        {
//            var node = nodes[0];
//            var remainingNodes = nodes.Skip(1).ToList();
//            var proxy = remainingNodes.Count == 0 ? this.CreateStaticLeafProxy(type, node) : this.CreateStaticChainedProxy(type, node, remainingNodes, this);
//            if (proxy != null)
//                return proxy;

//            if (log.IsWarnEnabled)
//                log.WarnFormat("Unable to bind: Not found the \"{0}\" member on the \"{1}\" type.", (nodes[0] as MemberNode).Name, type.Name);

//            return null;
//        }

//        public virtual IObjectSourceProxy CreateProxy(object source, List<IPathNode> nodes)
//        {
//            if (source == null)
//                return new EmptyObjectSourceProxy();

//            var node = nodes[0];
//            var remainingNodes = nodes.Skip(1).ToList();
//            var proxy = remainingNodes.Count == 0 ? this.CreateLeafProxy(source, node) : this.CreateChainedProxy(source, node, remainingNodes, this);
//            if (proxy != null)
//                return proxy;

//            if (log.IsWarnEnabled)
//                log.WarnFormat("Unable to bind: Not found the \"{0}\" member on the \"{1}\" type.", (nodes[0] as MemberNode).Name, source.GetType().Name);

//            return null;
//        }

//        protected virtual IObjectSourceProxy CreateChainedProxy(object source, IPathNode node, List<IPathNode> remainingNodes, IObjectSourceProxyFactory factory)
//        {
//            var indexedNode = node as IndexedNode;
//            if (indexedNode != null)
//            {
//                var itemPropertyInfo = this.FindItemPropertyInfo(source.GetType());
//                if (itemPropertyInfo == null)
//                    return null;

//                return new ChainedItemObjectSourceProxy(source, itemPropertyInfo, remainingNodes, factory, indexedNode.Value);
//            }

//            var memberNode = node as MemberNode;
//            if (memberNode == null)
//                return null;

//            var memberInfo = memberNode.MemberInfo;
//            if (memberInfo == null)
//                memberInfo = source.GetType().FindFirstMemberInfo(memberNode.Name);

//            if (memberInfo == null || memberInfo.IsStatic())
//                return null;

//            var propertyInfo = memberInfo as PropertyInfo;
//            if (propertyInfo != null)
//            {
//                var propertyType = propertyInfo.PropertyType;

//                if (typeof(IObservableProperty).IsAssignableFrom(propertyType))
//                {
//                    IProxyPropertyInfo proxyPropertyInfo = propertyInfo.AsProxy();
//                    object observableValue = proxyPropertyInfo.GetValue(source);
//                    if (observableValue == null)
//                        return null;

//                    return new ChainedObservablePropertyObjectSourceProxy(source, (IObservableProperty)observableValue, remainingNodes, factory);
//                }
//                else
//                {
//                    return new ChainedPropertyObjectSourceProxy(source, propertyInfo, remainingNodes, factory);
//                }
//            }

//            var fieldInfo = memberInfo as FieldInfo;
//            if (fieldInfo != null)
//            {
//                var fieldType = fieldInfo.FieldType;

//                if (typeof(IObservableProperty).IsAssignableFrom(fieldType))
//                {
//                    IProxyFieldInfo proxyFieldInfo = fieldInfo.AsProxy();
//                    object observableValue = proxyFieldInfo.GetValue(source);
//                    if (observableValue == null)
//                        return null;

//                    return new ChainedObservablePropertyObjectSourceProxy(source, (IObservableProperty)observableValue, remainingNodes, factory);
//                }
//                else
//                {
//                    return new ChainedFieldObjectSourceProxy(source, fieldInfo, remainingNodes, factory);
//                }
//            }
//            return null;
//        }

//        protected virtual IObjectSourceProxy CreateLeafProxy(object source, IPathNode node)
//        {
//            var indexedNode = node as IndexedNode;
//            if (indexedNode != null)
//            {
//                var itemPropertyInfo = this.FindItemPropertyInfo(source.GetType());
//                if (itemPropertyInfo == null)
//                    return null;

//                return new LeafItemObjectSourceProxy(source, itemPropertyInfo, indexedNode.Value);
//            }

//            var memberNode = node as MemberNode;
//            if (memberNode == null)
//                return null;

//            var memberInfo = memberNode.MemberInfo;
//            if (memberInfo == null)
//                memberInfo = source.GetType().FindFirstMemberInfo(memberNode.Name);

//            if (memberInfo == null || memberInfo.IsStatic())
//                return null;

//            var propertyInfo = memberInfo as PropertyInfo;
//            if (propertyInfo != null)
//            {
//                var propertyType = propertyInfo.PropertyType;

//                if (typeof(IObservableProperty).IsAssignableFrom(propertyType))
//                {
//                    IProxyPropertyInfo proxyPropertyInfo = propertyInfo.AsProxy();
//                    object observableValue = proxyPropertyInfo.GetValue(source);
//                    if (observableValue == null)
//                        return null;

//                    return new ObservablePropertyObjectSourceProxy(source, (IObservableProperty)observableValue);
//                }
//                else
//                {
//                    return new LeafPropertyObjectSourceProxy(source, propertyInfo);
//                }
//            }

//            var fieldInfo = memberInfo as FieldInfo;
//            if (fieldInfo != null)
//            {
//                if (typeof(IObservableProperty).IsAssignableFrom(fieldInfo.FieldType))
//                {
//                    IProxyFieldInfo proxyFieldInfo = fieldInfo.AsProxy();
//                    object observableValue = proxyFieldInfo.GetValue(source);
//                    if (observableValue == null)
//                        return null;

//                    return new ObservablePropertyObjectSourceProxy(source, (IObservableProperty)observableValue);
//                }
//                else
//                {
//                    return new LeafFieldObjectSourceProxy(source, fieldInfo);
//                }
//            }

//            var methodInfo = memberInfo as MethodInfo;
//            if (methodInfo != null && methodInfo.ReturnType.Equals(typeof(void)))
//                return new VoidMethodObjectSourceProxy(source, methodInfo);

//            return null;
//        }

//        protected virtual IObjectSourceProxy CreateStaticChainedProxy(Type type, IPathNode node, List<IPathNode> remainingNodes, IObjectSourceProxyFactory factory)
//        {
//            var memberNode = node as MemberNode;
//            if (memberNode == null)
//                return null;

//            var memberInfo = memberNode.MemberInfo;
//            if (memberInfo == null)
//                memberInfo = this.FindStaticMemberInfo(type, memberNode.Name);

//            if (memberInfo == null)
//                return null;

//            var propertyInfo = memberInfo as PropertyInfo;
//            if (propertyInfo != null)
//            {
//                var propertyType = propertyInfo.PropertyType;

//                if (typeof(IObservableProperty).IsAssignableFrom(propertyType))
//                {
//                    IProxyPropertyInfo proxyPropertyInfo = propertyInfo.AsProxy();
//                    object observableValue = proxyPropertyInfo.GetValue(null);
//                    if (observableValue == null)
//                        return null;

//                    return new ChainedObservablePropertyObjectSourceProxy((IObservableProperty)observableValue, remainingNodes, factory);
//                }
//                else
//                {
//                    return new ChainedPropertyObjectSourceProxy(propertyInfo, remainingNodes, factory);
//                }
//            }

//            var fieldInfo = memberInfo as FieldInfo;
//            if (fieldInfo != null)
//            {
//                var fieldType = fieldInfo.FieldType;

//                if (typeof(IObservableProperty).IsAssignableFrom(fieldType))
//                {
//                    IProxyFieldInfo proxyFieldInfo = fieldInfo.AsProxy();
//                    object observableValue = proxyFieldInfo.GetValue(null);
//                    if (observableValue == null)
//                        return null;

//                    return new ChainedObservablePropertyObjectSourceProxy((IObservableProperty)observableValue, remainingNodes, factory);
//                }
//                else
//                {
//                    return new ChainedFieldObjectSourceProxy(fieldInfo, remainingNodes, factory);
//                }
//            }
//            return null;
//        }

//        protected virtual IObjectSourceProxy CreateStaticLeafProxy(Type type, IPathNode node)
//        {
//            var memberNode = node as MemberNode;
//            if (memberNode == null)
//                return null;

//            var memberInfo = memberNode.MemberInfo;
//            if (memberInfo == null)
//                memberInfo = this.FindStaticMemberInfo(type, memberNode.Name);

//            if (memberInfo == null)
//                return null;

//            var propertyInfo = memberInfo as PropertyInfo;
//            if (propertyInfo != null)
//            {
//                if (typeof(IObservableProperty).IsAssignableFrom(propertyInfo.PropertyType))
//                {
//                    IProxyPropertyInfo proxyPropertyInfo = propertyInfo.AsProxy();
//                    object observableValue = proxyPropertyInfo.GetValue(null);
//                    if (observableValue == null)
//                        throw new ArgumentNullException();

//                    return new ObservablePropertyObjectSourceProxy((IObservableProperty)observableValue);
//                }
//                else
//                {
//                    return new LeafPropertyObjectSourceProxy(null, propertyInfo);
//                }
//            }

//            var fieldInfo = memberInfo as FieldInfo;
//            if (fieldInfo != null)
//            {
//                if (typeof(IObservableProperty).IsAssignableFrom(fieldInfo.FieldType))
//                {
//                    IProxyFieldInfo proxyFieldInfo = fieldInfo.AsProxy();
//                    object observableValue = proxyFieldInfo.GetValue(null);
//                    if (observableValue == null)
//                        throw new ArgumentNullException();

//                    return new ObservablePropertyObjectSourceProxy((IObservableProperty)observableValue);
//                }
//                else
//                {
//                    return new LeafFieldObjectSourceProxy(fieldInfo);
//                }
//            }

//            var methodInfo = memberInfo as MethodInfo;
//            if (methodInfo != null && methodInfo.ReturnType.Equals(typeof(void)))
//            {
//                return new VoidMethodObjectSourceProxy(methodInfo);
//            }

//            return null;
//        }

//        protected PropertyInfo FindItemPropertyInfo(Type type)
//        {
//            return type.GetProperty("Item");
//        }

//        protected MemberInfo FindStaticMemberInfo(Type type, string name)
//        {
//            return type.FindFirstMemberInfo(name, BindingFlags.Public | BindingFlags.Static);
//        }
//    }
//}
