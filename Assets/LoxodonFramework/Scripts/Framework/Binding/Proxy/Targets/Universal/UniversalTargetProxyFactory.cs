using System;
using System.Reflection;

using Loxodon.Log;

namespace Loxodon.Framework.Binding.Proxy.Targets
{
    public class UniversalTargetProxyFactory : AbstractTargetProxyFactory
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(UniversalTargetProxyFactory));

        protected override bool TryCreateProxy(object target, BindingDescription description, out ITargetProxy proxy)
        {
            proxy = null;
            Type type = target.GetType();
            MemberInfo memberInfo = type.FindFirstMemberInfo(description.TargetName);
            if (memberInfo == null)
                memberInfo = type.FindFirstMemberInfo(description.TargetName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (memberInfo == null)
                return false;

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                proxy = new PropertyTargetProxy(target, propertyInfo);
                return true;
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                proxy = new FieldTargetProxy(target, fieldInfo);
                return true;
            }

            var eventInfo = memberInfo as EventInfo;
            if (eventInfo != null)
            {
                proxy = new EventTargetProxy(target, eventInfo);
                return true;
            }

            var methodInfo = memberInfo as MethodInfo;
            if (methodInfo != null)
            {
                proxy = new VoidMethodTargetProxy(target, methodInfo);
                return true;
            }

            return false;
        }
    }
}
