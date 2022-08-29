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

#if UNITY_2019_1_OR_NEWER
using Loxodon.Framework.Binding.Reflection;
using Loxodon.Framework.Observables;
using System;
using System.Reflection;
using UnityEngine.UIElements;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class VisualElementProxyFactory : ITargetProxyFactory
    {
        public ITargetProxy CreateProxy(object target, BindingDescription description)
        {
            if (!target.GetType().IsSubclassOfGenericTypeDefinition(typeof(INotifyValueChanged<>)))
                return null;

            if ("RegisterValueChangedCallback".Equals(description.TargetName))
                return this.CreateValueChangedEventProxy(target);

            IProxyType type = description.TargetType != null ? description.TargetType.AsProxy() : target.GetType().AsProxy();
            IProxyMemberInfo memberInfo = type.GetMember(description.TargetName);
            if (memberInfo == null)
                memberInfo = type.GetMember(description.TargetName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (memberInfo == null)
                throw new MissingMemberException(type.Type.FullName, description.TargetName);

            var propertyInfo = memberInfo as IProxyPropertyInfo;
            if (propertyInfo != null)
            {
                if (typeof(IObservableProperty).IsAssignableFrom(propertyInfo.ValueType))
                    return null;

                if (typeof(Clickable).IsAssignableFrom(propertyInfo.ValueType))
                {
                    //Event Type
                    object clickable = propertyInfo.GetValue(target);
                    if (clickable == null)
                        throw new NullReferenceException(propertyInfo.Name);

                    return new ClickableEventProxy(target, (Clickable)clickable);
                }

                //Other Property Type
                if (!"RegisterValueChangedCallback".Equals(description.UpdateTrigger))/* by UniversalTargetProxyFactory */
                    return null;

                return CreateVisualElementPropertyProxy(target, propertyInfo);
            }

            var fieldInfo = memberInfo as IProxyFieldInfo;
            if (fieldInfo != null)
            {
                if (typeof(IObservableProperty).IsAssignableFrom(fieldInfo.ValueType))
                    return null;

                if (typeof(Clickable).IsAssignableFrom(fieldInfo.ValueType))
                {
                    //Event Type
                    object clickable = fieldInfo.GetValue(target);
                    if (clickable == null)
                        throw new NullReferenceException(fieldInfo.Name);

                    return new ClickableEventProxy(target, (Clickable)clickable);
                }

                //Other Property Type
                if (!"RegisterValueChangedCallback".Equals(description.UpdateTrigger))/* by UniversalTargetProxyFactory */
                    return null;

                return CreateVisualElementFieldProxy(target, fieldInfo);
            }

            return null;
        }

        protected virtual ITargetProxy CreateValueChangedEventProxy(object target)
        {
            var propertyInfo = target.GetType().GetProperty("value");
            Type type = propertyInfo.PropertyType;
#if NETFX_CORE
            TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(type);
#else
            TypeCode typeCode = Type.GetTypeCode(type);
#endif

            switch (typeCode)
            {
                case TypeCode.String: return new ValueChangedEventProxy<string>((INotifyValueChanged<string>)target);
                case TypeCode.Boolean: return new ValueChangedEventProxy<bool>((INotifyValueChanged<bool>)target);
                case TypeCode.SByte: return new ValueChangedEventProxy<sbyte>((INotifyValueChanged<sbyte>)target);
                case TypeCode.Byte: return new ValueChangedEventProxy<byte>((INotifyValueChanged<byte>)target);
                case TypeCode.Int16: return new ValueChangedEventProxy<short>((INotifyValueChanged<short>)target);
                case TypeCode.UInt16: return new ValueChangedEventProxy<ushort>((INotifyValueChanged<ushort>)target);
                case TypeCode.Int32: return new ValueChangedEventProxy<int>((INotifyValueChanged<int>)target);
                case TypeCode.UInt32: return new ValueChangedEventProxy<uint>((INotifyValueChanged<uint>)target);
                case TypeCode.Int64: return new ValueChangedEventProxy<long>((INotifyValueChanged<long>)target);
                case TypeCode.UInt64: return new ValueChangedEventProxy<ulong>((INotifyValueChanged<ulong>)target);
                case TypeCode.Char: return new ValueChangedEventProxy<char>((INotifyValueChanged<char>)target);
                case TypeCode.Single: return new ValueChangedEventProxy<float>((INotifyValueChanged<float>)target);
                case TypeCode.Double: return new ValueChangedEventProxy<double>((INotifyValueChanged<double>)target);
                case TypeCode.Decimal: return new ValueChangedEventProxy<decimal>((INotifyValueChanged<decimal>)target);
                case TypeCode.DateTime: return new ValueChangedEventProxy<DateTime>((INotifyValueChanged<DateTime>)target);
                case TypeCode.Object:
                default:
                    {
                        try
                        {
                            return (ITargetProxy)Activator.CreateInstance(typeof(ValueChangedEventProxy<>).MakeGenericType(type), target);
                        }
                        catch (Exception e)
                        {
                            throw new NotSupportedException("", e);
                        }
                    }
            }
        }

        protected virtual ITargetProxy CreateVisualElementPropertyProxy(object target, IProxyPropertyInfo propertyInfo)
        {
            Type type = propertyInfo.ValueType;
#if NETFX_CORE
            TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(type);
#else
            TypeCode typeCode = Type.GetTypeCode(type);
#endif

            switch (typeCode)
            {
                case TypeCode.String: return new VisualElementPropertyProxy<string>(target, propertyInfo);
                case TypeCode.Boolean: return new VisualElementPropertyProxy<bool>(target, propertyInfo);
                case TypeCode.SByte: return new VisualElementPropertyProxy<sbyte>(target, propertyInfo);
                case TypeCode.Byte: return new VisualElementPropertyProxy<byte>(target, propertyInfo);
                case TypeCode.Int16: return new VisualElementPropertyProxy<short>(target, propertyInfo);
                case TypeCode.UInt16: return new VisualElementPropertyProxy<ushort>(target, propertyInfo);
                case TypeCode.Int32: return new VisualElementPropertyProxy<int>(target, propertyInfo);
                case TypeCode.UInt32: return new VisualElementPropertyProxy<uint>(target, propertyInfo);
                case TypeCode.Int64: return new VisualElementPropertyProxy<long>(target, propertyInfo);
                case TypeCode.UInt64: return new VisualElementPropertyProxy<ulong>(target, propertyInfo);
                case TypeCode.Char: return new VisualElementPropertyProxy<char>(target, propertyInfo);
                case TypeCode.Single: return new VisualElementPropertyProxy<float>(target, propertyInfo);
                case TypeCode.Double: return new VisualElementPropertyProxy<double>(target, propertyInfo);
                case TypeCode.Decimal: return new VisualElementPropertyProxy<decimal>(target, propertyInfo);
                case TypeCode.DateTime: return new VisualElementPropertyProxy<DateTime>(target, propertyInfo);
                case TypeCode.Object:
                default:
                    {
                        try
                        {
                            return (ITargetProxy)Activator.CreateInstance(typeof(VisualElementPropertyProxy<>).MakeGenericType(type), target, propertyInfo);
                        }
                        catch (Exception e)
                        {
                            throw new NotSupportedException("", e);
                        }
                    }
            }
        }

        protected virtual ITargetProxy CreateVisualElementFieldProxy(object target, IProxyFieldInfo fieldInfo)
        {
            Type type = fieldInfo.ValueType;
#if NETFX_CORE
            TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(type);
#else
            TypeCode typeCode = Type.GetTypeCode(type);
#endif

            switch (typeCode)
            {
                case TypeCode.String: return new VisualElementFieldProxy<string>(target, fieldInfo);
                case TypeCode.Boolean: return new VisualElementFieldProxy<bool>(target, fieldInfo);
                case TypeCode.SByte: return new VisualElementFieldProxy<sbyte>(target, fieldInfo);
                case TypeCode.Byte: return new VisualElementFieldProxy<byte>(target, fieldInfo);
                case TypeCode.Int16: return new VisualElementFieldProxy<short>(target, fieldInfo);
                case TypeCode.UInt16: return new VisualElementFieldProxy<ushort>(target, fieldInfo);
                case TypeCode.Int32: return new VisualElementFieldProxy<int>(target, fieldInfo);
                case TypeCode.UInt32: return new VisualElementFieldProxy<uint>(target, fieldInfo);
                case TypeCode.Int64: return new VisualElementFieldProxy<long>(target, fieldInfo);
                case TypeCode.UInt64: return new VisualElementFieldProxy<ulong>(target, fieldInfo);
                case TypeCode.Char: return new VisualElementFieldProxy<char>(target, fieldInfo);
                case TypeCode.Single: return new VisualElementFieldProxy<float>(target, fieldInfo);
                case TypeCode.Double: return new VisualElementFieldProxy<double>(target, fieldInfo);
                case TypeCode.Decimal: return new VisualElementFieldProxy<decimal>(target, fieldInfo);
                case TypeCode.DateTime: return new VisualElementFieldProxy<DateTime>(target, fieldInfo);
                case TypeCode.Object:
                default:
                    {
                        try
                        {
                            return (ITargetProxy)Activator.CreateInstance(typeof(VisualElementFieldProxy<>).MakeGenericType(type), target, fieldInfo);
                        }
                        catch (Exception e)
                        {
                            throw new NotSupportedException("", e);
                        }
                    }
            }
        }

    }
}
#endif