using System;
using System.Collections.Generic;

using Loxodon.Framework.Binding.Paths;
using Loxodon.Framework.Binding.Proxy.Sources.Object;
using Loxodon.Framework.Binding.Expressions;
using Loxodon.Framework.Binding.Proxy.Sources.Expressions;

namespace Loxodon.Framework.Binding.Proxy.Sources.Expressions
{
    public class ExpressionSourceProxyFactory : TypedSourceProxyFactory<ExpressionSourceDescription>
    {
        private ISourceProxyFactory factory;
        private IExpressionPathFinder pathFinder;
        public ExpressionSourceProxyFactory(ISourceProxyFactory factory, IExpressionPathFinder pathFinder)
        {
            this.factory = factory;
            this.pathFinder = pathFinder;
        }

        protected override bool TryCreateProxy(object source, ExpressionSourceDescription description, out ISourceProxy proxy)
        {
            proxy = null;
            var expression = description.Expression;
            List<ISourceProxy> list = new List<ISourceProxy>();
            if (!description.IsStatic)
            {
                List<Path> paths = this.pathFinder.FindPaths(expression);
                foreach (Path path in paths)
                {
                    ISourceProxy innerProxy = this.factory.CreateProxy(source, new ObjectSourceDescription() { Path = path });
                    if (innerProxy != null)
                        list.Add(innerProxy);
                }
            }
#if UNITY_IOS
            Func<object[], object> del = expression.DynamicCompile();
            proxy = new ExpressionSourceProxy(description.IsStatic ? null : source, del, description.ReturnType, list);
#else
            try
            {
                var del = expression.Compile();
                Type returnType = del.ReturnType();
                Type parameterType = del.ParameterType();
                if (parameterType != null)
                {
                    proxy = (ISourceProxy)Activator.CreateInstance(typeof(ExpressionSourceProxy<,>).MakeGenericType(parameterType, returnType), source, del, list);
                }
                else
                {
                    proxy = (ISourceProxy)Activator.CreateInstance(typeof(ExpressionSourceProxy<>).MakeGenericType(returnType), del);
                }
            }
            catch (Exception)
            {
                //JIT Exception
                Func<object[], object> del = expression.DynamicCompile();
                proxy = new ExpressionSourceProxy(description.IsStatic ? null : source, del, description.ReturnType, list);
            }
#endif
            if (proxy != null)
                return true;

            return false;
        }
    }
}
