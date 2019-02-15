using System;
using System.Reflection;
using System.Linq.Expressions;

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
            var field = info as FieldInfo;
            if (field != null)
                return field.GetValue(root);

            var property = info as PropertyInfo;
            if (property != null)
            {
                var method = property.GetGetMethod();
                if (method != null)
                    return method.Invoke(root, null);
            }

            throw new NotSupportedException("Bad MemberInfo type.");
        }

        internal static void Set(this MemberInfo info, object root, object value)
        {
            var field = info as FieldInfo;
            if (field != null)
            {
                field.SetValue(root, value);
                return;
            }
            var property = info as PropertyInfo;
            if (property != null)
            {
                var method = property.GetSetMethod();
                if (method != null)
                    method.Invoke(root, new object[] { value });
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
