using System;
using System.Reflection;

using Loxodon.Log;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Binding.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class ObservablePropertyTargetProxyFactory : AbstractTargetProxyFactory
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(ObservablePropertyTargetProxyFactory));

        protected override bool TryCreateProxy(object target, BindingDescription description, out ITargetProxy proxy)
        {
            proxy = null;
            Type type = target.GetType();
            MemberInfo memberInfo = type.FindFirstMemberInfo(description.TargetName);
            if (memberInfo == null)
                memberInfo = type.FindFirstMemberInfo(description.TargetName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (memberInfo == null)
                return false;

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null && typeof(IObservableProperty).IsAssignableFrom(fieldInfo.FieldType))
            {
                var fieldType = fieldInfo.FieldType;
                var proxyFieldInfo = fieldInfo.AsProxy();
                object observableValue = proxyFieldInfo.GetValue(target);
                proxy = new ObservablePropertyTargetProxy(target, (IObservableProperty)observableValue);
                return true;
            }

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null && typeof(IObservableProperty).IsAssignableFrom(propertyInfo.PropertyType))
            {
                var propertyType = propertyInfo.PropertyType;
                var proxyPropertyInfo = propertyInfo.AsProxy();
                object observableValue = proxyPropertyInfo.GetValue(target);
                proxy = new ObservablePropertyTargetProxy(target, (IObservableProperty)observableValue);
                return true;
            }

            return false;
        }
    }
}
