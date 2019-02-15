using System.Reflection;

using Loxodon.Framework.Binding.Paths;
using Loxodon.Framework.Interactivity;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public class InteractionRequestSourceProxyFactory : ISpecificTypeObjectSourceProxyFactory
    {
        public bool TryCreateProxy(object source, PathToken token, IObjectSourceProxyFactory factory, out IObjectSourceProxy proxy)
        {
            proxy = null;
            if (source == null || token.HasNext() || !(token.Current is MemberNode))
                return false;

            MemberInfo memberInfo = token.GetMemberInfo(source);
            if (memberInfo == null)
                return false;

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null && typeof(IInteractionRequest).IsAssignableFrom(fieldInfo.FieldType))
            {
                proxy = new InteractionRequestFieldObjectSourceProxy(source, fieldInfo);
                return true;
            }

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null && typeof(IInteractionRequest).IsAssignableFrom(propertyInfo.PropertyType))
            {
                proxy = new InteractionRequestPropertyObjectSourceProxy(source, propertyInfo);
                return true;
            }

            return false;
        }
    }
}
