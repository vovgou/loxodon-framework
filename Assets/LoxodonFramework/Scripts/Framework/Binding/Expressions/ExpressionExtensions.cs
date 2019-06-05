using System;
using System.Reflection;
using System.Linq.Expressions;
using Loxodon.Framework.Binding.Reflection;

namespace Loxodon.Framework.Binding.Expressions
{
    public static class ExpressionExtensions
    {
        public static Func<object[], object> DynamicCompile(this LambdaExpression expr)
        {
            return (Func<object[], object>)((ConstantExpression)new EvaluatingVisitor().Visit(expr)).Value;
        }

        public static Func<object[], object> DynamicCompile<T>(this Expression<T> expr)
        {
            return DynamicCompile((LambdaExpression)expr);
        }

        internal static object Get(this MemberInfo info, object root)
        {
            var fieldInfo = info as FieldInfo;
            if (fieldInfo != null)
            {
                var proxyFieldInfo = fieldInfo.AsProxy();
                if (proxyFieldInfo != null)
                    return proxyFieldInfo.GetValue(root);

                return fieldInfo.GetValue(root);
            }

            var propertyInfo = info as PropertyInfo;
            if (propertyInfo != null)
            {
                var proxyPropertyInfo = propertyInfo.AsProxy();
                if (proxyPropertyInfo != null)
                    return proxyPropertyInfo.GetValue(root);

                var method = propertyInfo.GetGetMethod();
                if (method != null)
                    return method.Invoke(root, null);
            }

            throw new NotSupportedException("Bad MemberInfo type.");
        }

        internal static void Set(this MemberInfo info, object root, object value)
        {
            var fieldInfo = info as FieldInfo;
            if (fieldInfo != null)
            {
                var proxyFieldInfo = fieldInfo.AsProxy();
                if (proxyFieldInfo != null)
                {
                    proxyFieldInfo.SetValue(root, value);
                }
                else
                {
                    fieldInfo.SetValue(root, value);
                }
                return;
            }
            var propertyInfo = info as PropertyInfo;
            if (propertyInfo != null)
            {
                var proxyPropertyInfo = propertyInfo.AsProxy();
                if (proxyPropertyInfo != null)
                {
                    proxyPropertyInfo.SetValue(root, value);
                }
                else
                {
                    var method = propertyInfo.GetSetMethod();
                    if (method != null)
                        method.Invoke(root, new object[] { value });
                }
                return;
            }
            throw new NotSupportedException("Bad MemberInfo type.");
        }

        internal static MethodInfo GetMethod(this Type type, string name, int genericParamLength)
        {
            foreach (MethodInfo info in type.GetMethods())
            {
                if (!info.Name.Equals(name))
                    continue;

                Type[] argumentTypes = info.GetGenericArguments();
                if (argumentTypes.Length != genericParamLength)
                    continue;
                return info;
            }
            return null;
        }
    }
}
