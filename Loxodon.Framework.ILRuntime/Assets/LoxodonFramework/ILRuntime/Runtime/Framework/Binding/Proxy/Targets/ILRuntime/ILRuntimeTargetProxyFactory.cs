using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using Loxodon.Framework.Binding.Reflection;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using System;
using System.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class ILRuntimeTargetProxyFactory : ITargetProxyFactory
    {
        public ITargetProxy CreateProxy(object target, BindingDescription description)
        {
            if (target == null || !(target is ILTypeInstance || target is CrossBindingAdaptorType))
                return null;

            ILTypeInstance typeInstance = target as ILTypeInstance;
            if (typeInstance == null)
                typeInstance = (target as CrossBindingAdaptorType).ILInstance;

            IProxyType type = typeInstance.Type.ReflectionType.AsProxy();
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
    }
}
