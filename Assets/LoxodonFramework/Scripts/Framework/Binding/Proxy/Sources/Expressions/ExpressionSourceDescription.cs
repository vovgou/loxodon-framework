using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Loxodon.Framework.Binding.Proxy.Sources.Expressions
{
    public class ExpressionSourceDescription : SourceDescription
    {
        private LambdaExpression expression;

        private Type returnType;

        public ExpressionSourceDescription()
        {
        }

        public LambdaExpression Expression
        {
            get { return this.expression; }
            set
            {
                this.expression = value;

                Type[] types = expression.GetType().GetGenericArguments();
                var delType = types[0];

                if (!typeof(Delegate).IsAssignableFrom(delType))
                    throw new NotSupportedException();

                MethodInfo info = delType.GetMethod("Invoke");

                this.returnType = info.ReturnType;

                ParameterInfo[] parameters = info.GetParameters();
                this.IsStatic = (parameters == null || parameters.Length <= 0) ? true : false;
            }
        }

        public Type ReturnType { get { return this.returnType; } }

        public override string ToString()
        {
            return this.expression == null ? "Expression:null" : "Expression:" + this.expression.ToString();
        }
    }
}
