using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Events;

using Loxodon.Log;
using Loxodon.Framework.Binding.Reflection;
using Loxodon.Framework.Observables;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class UnityTargetProxyFactory : AbstractTargetProxyFactory
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(UnityTargetProxyFactory));

        protected override bool TryCreateProxy(object target, BindingDescription description, out ITargetProxy proxy)
        {
            proxy = null;
            Type type = target.GetType();
            MemberInfo memberInfo = type.FindFirstMemberInfo(description.TargetName);
            if (memberInfo == null)
                return false;

            UnityEventBase updateTrigger = null;
            if (!string.IsNullOrEmpty(description.UpdateTrigger))
            {
                var updateTriggerPropertyInfo = type.GetProperty(description.UpdateTrigger);
                if (updateTriggerPropertyInfo != null)
                    updateTrigger = updateTriggerPropertyInfo.AsProxy().GetValue(target) as UnityEventBase;

                if (updateTriggerPropertyInfo == null)
                {
                    var updateTriggerFieldInfo = type.GetField(description.UpdateTrigger);
                    if (updateTriggerFieldInfo != null)
                        updateTrigger = updateTriggerFieldInfo.AsProxy().GetValue(target) as UnityEventBase;
                }
            }

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                if (typeof(IObservableProperty).IsAssignableFrom(propertyInfo.PropertyType))
                    return false;

                if (typeof(UnityEventBase).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    //Event Type
                    var proxyPropertyInfo = propertyInfo.AsProxy();
                    object unityEvent = proxyPropertyInfo.GetValue(target);
                    Type[] paramTypes = GetUnityEventParametersType(propertyInfo.PropertyType);
                    proxy = CreateUnityEventProxy(target, (UnityEventBase)unityEvent, paramTypes);
                    if (proxy != null)
                        return true;
                    return false;
                }

                //Other Property Type
                if (updateTrigger == null)/* by UniversalTargetProxyFactory */
                    return false;

                proxy = CreateUnityPropertyProxy(target, propertyInfo, updateTrigger);
                if (proxy != null)
                    return true;

                return false;
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                if (typeof(IObservableProperty).IsAssignableFrom(fieldInfo.FieldType))
                    return false;

                if (typeof(UnityEventBase).IsAssignableFrom(fieldInfo.FieldType))
                {
                    //Event Type
                    var proxyFieldInfo = fieldInfo.AsProxy();
                    object unityEvent = proxyFieldInfo.GetValue(target);
                    Type[] paramTypes = GetUnityEventParametersType(proxyFieldInfo.FieldType);
                    proxy = CreateUnityEventProxy(target, (UnityEventBase)unityEvent, paramTypes);
                    if (proxy != null)
                        return true;
                    return false;
                }

                //Other Property Type
                if (updateTrigger == null)/* by UniversalTargetProxyFactory */
                    return false;

                proxy = CreateUnityFieldProxy(target, fieldInfo, updateTrigger);
                if (proxy != null)
                    return true;

                return false;
            }

            proxy = null;
            return false;
        }

        protected virtual ITargetProxy CreateUnityPropertyProxy(object target, PropertyInfo propertyInfo, UnityEventBase updateTrigger)
        {
            Type type = propertyInfo.PropertyType;
#if NETFX_CORE
            TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(type);
#else
            TypeCode typeCode = Type.GetTypeCode(type);
#endif

            switch (typeCode)
            {
                case TypeCode.String: return new UnityPropertyProxy<string>(target, propertyInfo, (UnityEvent<string>)updateTrigger);
                case TypeCode.Boolean: return new UnityPropertyProxy<bool>(target, propertyInfo, (UnityEvent<bool>)updateTrigger);
                case TypeCode.SByte: return new UnityPropertyProxy<sbyte>(target, propertyInfo, (UnityEvent<sbyte>)updateTrigger);
                case TypeCode.Byte: return new UnityPropertyProxy<byte>(target, propertyInfo, (UnityEvent<byte>)updateTrigger);
                case TypeCode.Int16: return new UnityPropertyProxy<short>(target, propertyInfo, (UnityEvent<short>)updateTrigger);
                case TypeCode.UInt16: return new UnityPropertyProxy<ushort>(target, propertyInfo, (UnityEvent<ushort>)updateTrigger);
                case TypeCode.Int32: return new UnityPropertyProxy<int>(target, propertyInfo, (UnityEvent<int>)updateTrigger);
                case TypeCode.UInt32: return new UnityPropertyProxy<uint>(target, propertyInfo, (UnityEvent<uint>)updateTrigger);
                case TypeCode.Int64: return new UnityPropertyProxy<long>(target, propertyInfo, (UnityEvent<long>)updateTrigger);
                case TypeCode.UInt64: return new UnityPropertyProxy<ulong>(target, propertyInfo, (UnityEvent<ulong>)updateTrigger);
                case TypeCode.Char: return new UnityPropertyProxy<char>(target, propertyInfo, (UnityEvent<char>)updateTrigger);
                case TypeCode.Single: return new UnityPropertyProxy<float>(target, propertyInfo, (UnityEvent<float>)updateTrigger);
                case TypeCode.Double: return new UnityPropertyProxy<double>(target, propertyInfo, (UnityEvent<double>)updateTrigger);
                case TypeCode.Decimal: return new UnityPropertyProxy<decimal>(target, propertyInfo, (UnityEvent<decimal>)updateTrigger);
                case TypeCode.DateTime: return new UnityPropertyProxy<DateTime>(target, propertyInfo, (UnityEvent<DateTime>)updateTrigger);
                case TypeCode.Object:
                default:
#if UNITY_IOS
                    throw new NotSupportedException();
#else
                    return (ITargetProxy)Activator.CreateInstance(typeof(UnityPropertyProxy<>).MakeGenericType(propertyInfo.PropertyType), target, propertyInfo, updateTrigger);
#endif
            }
        }

        protected virtual ITargetProxy CreateUnityFieldProxy(object target, FieldInfo fieldInfo, UnityEventBase updateTrigger)
        {
            Type type = fieldInfo.FieldType;
#if NETFX_CORE
            TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(type);
#else
            TypeCode typeCode = Type.GetTypeCode(type);
#endif

            switch (typeCode)
            {
                case TypeCode.String: return new UnityFieldProxy<string>(target, fieldInfo, (UnityEvent<string>)updateTrigger);
                case TypeCode.Boolean: return new UnityFieldProxy<bool>(target, fieldInfo, (UnityEvent<bool>)updateTrigger);
                case TypeCode.SByte: return new UnityFieldProxy<sbyte>(target, fieldInfo, (UnityEvent<sbyte>)updateTrigger);
                case TypeCode.Byte: return new UnityFieldProxy<byte>(target, fieldInfo, (UnityEvent<byte>)updateTrigger);
                case TypeCode.Int16: return new UnityFieldProxy<short>(target, fieldInfo, (UnityEvent<short>)updateTrigger);
                case TypeCode.UInt16: return new UnityFieldProxy<ushort>(target, fieldInfo, (UnityEvent<ushort>)updateTrigger);
                case TypeCode.Int32: return new UnityFieldProxy<int>(target, fieldInfo, (UnityEvent<int>)updateTrigger);
                case TypeCode.UInt32: return new UnityFieldProxy<uint>(target, fieldInfo, (UnityEvent<uint>)updateTrigger);
                case TypeCode.Int64: return new UnityFieldProxy<long>(target, fieldInfo, (UnityEvent<long>)updateTrigger);
                case TypeCode.UInt64: return new UnityFieldProxy<ulong>(target, fieldInfo, (UnityEvent<ulong>)updateTrigger);
                case TypeCode.Char: return new UnityFieldProxy<char>(target, fieldInfo, (UnityEvent<char>)updateTrigger);
                case TypeCode.Single: return new UnityFieldProxy<float>(target, fieldInfo, (UnityEvent<float>)updateTrigger);
                case TypeCode.Double: return new UnityFieldProxy<double>(target, fieldInfo, (UnityEvent<double>)updateTrigger);
                case TypeCode.Decimal: return new UnityFieldProxy<decimal>(target, fieldInfo, (UnityEvent<decimal>)updateTrigger);
                case TypeCode.DateTime: return new UnityFieldProxy<DateTime>(target, fieldInfo, (UnityEvent<DateTime>)updateTrigger);
                case TypeCode.Object:
                default:
#if UNITY_IOS
                    throw new NotSupportedException();
#else
                    return (ITargetProxy)Activator.CreateInstance(typeof(UnityFieldProxy<>).MakeGenericType(fieldInfo.FieldType), target, fieldInfo, updateTrigger);
#endif
            }
        }

        protected virtual ITargetProxy CreateUnityEventProxy(object target, UnityEventBase unityEvent, Type[] paramTypes)
        {
            ITargetProxy proxy = null;
            switch (paramTypes.Length)
            {
                case 0:
                    proxy = new UnityEventProxy(target, (UnityEvent)unityEvent);
                    break;
                case 1:
#if NETFX_CORE
                    TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(paramTypes[0]);
#else
                    TypeCode typeCode = Type.GetTypeCode(paramTypes[0]);
#endif
                    switch (typeCode)
                    {
                        case TypeCode.String:
                            proxy = new UnityEventProxy<string>(target, (UnityEvent<string>)unityEvent);
                            break;
                        case TypeCode.Boolean:
                            proxy = new UnityEventProxy<bool>(target, (UnityEvent<bool>)unityEvent);
                            break;
                        case TypeCode.SByte:
                            proxy = new UnityEventProxy<sbyte>(target, (UnityEvent<sbyte>)unityEvent);
                            break;
                        case TypeCode.Byte:
                            proxy = new UnityEventProxy<byte>(target, (UnityEvent<byte>)unityEvent);
                            break;
                        case TypeCode.Int16:
                            proxy = new UnityEventProxy<short>(target, (UnityEvent<short>)unityEvent);
                            break;
                        case TypeCode.UInt16:
                            proxy = new UnityEventProxy<ushort>(target, (UnityEvent<ushort>)unityEvent);
                            break;
                        case TypeCode.Int32:
                            proxy = new UnityEventProxy<int>(target, (UnityEvent<int>)unityEvent);
                            break;
                        case TypeCode.UInt32:
                            proxy = new UnityEventProxy<uint>(target, (UnityEvent<uint>)unityEvent);
                            break;
                        case TypeCode.Int64:
                            proxy = new UnityEventProxy<long>(target, (UnityEvent<long>)unityEvent);
                            break;
                        case TypeCode.UInt64:
                            proxy = new UnityEventProxy<ulong>(target, (UnityEvent<ulong>)unityEvent);
                            break;
                        case TypeCode.Char:
                            proxy = new UnityEventProxy<char>(target, (UnityEvent<char>)unityEvent);
                            break;
                        case TypeCode.Single:
                            proxy = new UnityEventProxy<float>(target, (UnityEvent<float>)unityEvent);
                            break;
                        case TypeCode.Double:
                            proxy = new UnityEventProxy<double>(target, (UnityEvent<double>)unityEvent);
                            break;
                        case TypeCode.Decimal:
                            proxy = new UnityEventProxy<decimal>(target, (UnityEvent<decimal>)unityEvent);
                            break;
                        case TypeCode.DateTime:
                            proxy = new UnityEventProxy<DateTime>(target, (UnityEvent<DateTime>)unityEvent);
                            break;
                        case TypeCode.Object:
                        default:
#if UNITY_IOS
                            throw new NotSupportedException();
#else
                            proxy = (ITargetProxy)Activator.CreateInstance(typeof(UnityEventProxy<>).MakeGenericType(paramTypes[0]), target, unityEvent);
                            break;
#endif
                    }
                    break;
                default:
                    throw new NotSupportedException("Too many parameters");
            }
            return proxy;
        }

        protected Type[] GetUnityEventParametersType(Type type)
        {
            MethodInfo info = type.GetMethod("Invoke");
            if (info == null)
                return new Type[0];

            ParameterInfo[] parameters = info.GetParameters();
            if (parameters == null || parameters.Length <= 0)
                return new Type[0];

            List<Type> types = new List<Type>();
            foreach (ParameterInfo parameter in parameters)
            {
                if (types.Contains(parameter.ParameterType))
                    continue;

                types.Add(parameter.ParameterType);
            }

            return types.ToArray();
        }
    }
}
