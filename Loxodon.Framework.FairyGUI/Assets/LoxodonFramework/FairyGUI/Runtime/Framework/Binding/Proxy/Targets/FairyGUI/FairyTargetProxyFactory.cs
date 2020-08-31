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

using FairyGUI;
using Loxodon.Framework.Binding.Reflection;
using Loxodon.Framework.Observables;
using System;
using System.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class FairyTargetProxyFactory : ITargetProxyFactory
    {
        public ITargetProxy CreateProxy(object target, BindingDescription description)
        {
            IProxyType type = target.GetType().AsProxy();
            IProxyMemberInfo memberInfo = type.GetMember(description.TargetName);
            if (memberInfo == null)
                memberInfo = type.GetMember(description.TargetName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (memberInfo == null)
                throw new MissingMemberException(type.Type.FullName, description.TargetName);

            EventListener updateTrigger = null;
            if (!string.IsNullOrEmpty(description.UpdateTrigger))
            {
                IProxyPropertyInfo updateTriggerPropertyInfo = type.GetProperty(description.UpdateTrigger);
                IProxyFieldInfo updateTriggerFieldInfo = updateTriggerPropertyInfo == null ? type.GetField(description.UpdateTrigger) : null;
                if (updateTriggerPropertyInfo != null)
                    updateTrigger = updateTriggerPropertyInfo.GetValue(target) as EventListener;

                if (updateTriggerFieldInfo != null)
                    updateTrigger = updateTriggerFieldInfo.GetValue(target) as EventListener;

                if (updateTriggerPropertyInfo == null && updateTriggerFieldInfo == null)
                    throw new MissingMemberException(type.Type.FullName, description.UpdateTrigger);

                //Other Property Type
                if (updateTrigger == null) /* by UniversalTargetProxyFactory */
                    return null;
            }

            var propertyInfo = memberInfo as IProxyPropertyInfo;
            if (propertyInfo != null)
            {
                if (typeof(IObservableProperty).IsAssignableFrom(propertyInfo.ValueType))
                    return null;

                if (typeof(EventListener).IsAssignableFrom(propertyInfo.ValueType))
                {
                    //Event Type
                    object listener = propertyInfo.GetValue(target);
                    if (listener == null)
                        throw new NullReferenceException(propertyInfo.Name);

                    return new FairyEventProxy(target, (EventListener)listener);
                }

                //Other Property Type
                if (updateTrigger == null)/* by UniversalTargetProxyFactory */
                    return null;

                return new FairyPropertyProxy(target, propertyInfo, updateTrigger);
            }

            var fieldInfo = memberInfo as IProxyFieldInfo;
            if (fieldInfo != null)
            {
                if (typeof(IObservableProperty).IsAssignableFrom(fieldInfo.ValueType))
                    return null;

                if (typeof(EventListener).IsAssignableFrom(fieldInfo.ValueType))
                {
                    //Event Type
                    object listener = fieldInfo.GetValue(target);
                    if (listener == null)
                        throw new NullReferenceException(fieldInfo.Name);

                    return new FairyEventProxy(target, (EventListener)listener);
                }

                //Other Property Type
                if (updateTrigger == null)/* by UniversalTargetProxyFactory */
                    return null;

                return new FairyFieldProxy(target, fieldInfo, updateTrigger);
            }

            return null;
        }

    }
}