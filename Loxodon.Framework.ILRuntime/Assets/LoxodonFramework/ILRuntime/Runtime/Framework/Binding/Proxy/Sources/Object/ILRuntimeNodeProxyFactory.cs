using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using Loxodon.Framework.Binding.Paths;
using Loxodon.Framework.Binding.Reflection;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using System;
using System.Collections;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public class ILRuntimeNodeProxyFactory : INodeProxyFactory
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(ILRuntimeNodeProxyFactory));

        public ISourceProxy Create(object source, PathToken token)
        {
            IPathNode node = token.Current;
            if (source == null || !(source is ILTypeInstance || source is CrossBindingAdaptorType))
                return null;

            return CreateProxy(source, node);
        }

        protected virtual ISourceProxy CreateProxy(object source, IPathNode node)
        {
            Type type = GetType(source);
            IProxyType proxyType = type.AsProxy();
            if (node is IndexedNode)
            {
                if (!(source is ICollection))
                    throw new ProxyException("Type \"{0}\" is not a collection and cannot be accessed by index \"{1}\".", proxyType.Type.Name, node.ToString());

                var itemInfo = proxyType.GetItem();
                if (itemInfo == null)
                    throw new MissingMemberException(proxyType.Type.FullName, "Item");

                var intIndexedNode = node as IntegerIndexedNode;
                if (intIndexedNode != null)
                    return new IntItemNodeProxy((ICollection)source, intIndexedNode.Value, itemInfo);

                var stringIndexedNode = node as StringIndexedNode;
                if (stringIndexedNode != null)
                    return new StringItemNodeProxy((ICollection)source, stringIndexedNode.Value, itemInfo);

                return null;
            }

            var memberNode = node as MemberNode;
            if (memberNode == null)
                return null;

            var memberInfo = proxyType.GetMember(memberNode.Name);
            if (memberInfo == null || memberInfo.IsStatic)
                throw new MissingMemberException(proxyType.Type.FullName, memberNode.Name);

            var proxyPropertyInfo = memberInfo as IProxyPropertyInfo;
            if (proxyPropertyInfo != null)
            {
                var valueType = proxyPropertyInfo.ValueType;
                if (typeof(IObservableProperty).IsAssignableFrom(valueType))
                {
                    object observableValue = proxyPropertyInfo.GetValue(source);
                    if (observableValue == null)
                        return null;

                    return new ObservableNodeProxy(source, (IObservableProperty)observableValue);
                }
                else if (typeof(IInteractionRequest).IsAssignableFrom(valueType))
                {
                    object request = proxyPropertyInfo.GetValue(source);
                    if (request == null)
                        return null;

                    return new InteractionNodeProxy(source, (IInteractionRequest)request);
                }
                else
                {
                    return new PropertyNodeProxy(source, proxyPropertyInfo);
                }
            }

            var proxyFieldInfo = memberInfo as IProxyFieldInfo;
            if (proxyFieldInfo != null)
            {
                var valueType = proxyFieldInfo.ValueType;
                if (typeof(IObservableProperty).IsAssignableFrom(valueType))
                {
                    object observableValue = proxyFieldInfo.GetValue(source);
                    if (observableValue == null)
                        return null;

                    return new ObservableNodeProxy(source, (IObservableProperty)observableValue);
                }
                else if (typeof(IInteractionRequest).IsAssignableFrom(valueType))
                {
                    object request = proxyFieldInfo.GetValue(source);
                    if (request == null)
                        return null;

                    return new InteractionNodeProxy(source, (IInteractionRequest)request);
                }
                else
                {
                    return new FieldNodeProxy(source, proxyFieldInfo);
                }
            }

            var methodInfo = memberInfo as IProxyMethodInfo;
            if (methodInfo != null && methodInfo.ReturnType.Equals(typeof(void)))
                return new MethodNodeProxy(source, methodInfo);

            var eventInfo = memberInfo as IProxyEventInfo;
            if (eventInfo != null)
                return new EventNodeProxy(source, eventInfo);

            return null;
        }

        protected Type GetType(object source)
        {
            ILTypeInstance typeInstance = source as ILTypeInstance;
            if (typeInstance != null)
                return typeInstance.Type.ReflectionType;

            CrossBindingAdaptorType adaptor = source as CrossBindingAdaptorType;
            if (adaptor != null)
                return adaptor.ILInstance.Type.ReflectionType;

            return source.GetType();
        }
    }
}
