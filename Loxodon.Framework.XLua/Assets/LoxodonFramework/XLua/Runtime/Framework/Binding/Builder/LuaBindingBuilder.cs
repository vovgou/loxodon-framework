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

using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Binding.Converters;
using Loxodon.Framework.Binding.Parameters;
using Loxodon.Framework.Binding.Proxy.Sources.Expressions;
using XLua;

namespace Loxodon.Framework.Binding.Builder
{
    [LuaCallCSharp]
    public class LuaBindingBuilder : BindingBuilderBase
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(LuaBindingBuilder));

        public LuaBindingBuilder(IBindingContext context, object target) : base(context, target)
        {
        }

        public LuaBindingBuilder For(string targetName, string updateTrigger = null)
        {
            this.description.TargetName = targetName;
            this.description.UpdateTrigger = updateTrigger;
            return this;
        }

        public LuaBindingBuilder To(string path)
        {
            this.SetMemberPath(path);
            return this;
        }

        public LuaBindingBuilder ToExpression(LuaFunction expression, params string[] paths)
        {
            if (this.description.Source != null)
                throw new BindingException("You cannot set the source path of a Fluent binding more than once");

            this.description.Source = new LuaExpressionSourceDescription()
            {
                Expression = expression,
                Paths = paths
            };

            return this;
        }

        public LuaBindingBuilder ToStatic(string path)
        {
            this.SetStaticMemberPath(path);
            return this;
        }

        public LuaBindingBuilder ToValue(object value)
        {
            this.SetLiteral(value);
            return this;
        }

        public LuaBindingBuilder TwoWay()
        {
            this.SetMode(BindingMode.TwoWay);
            return this;
        }

        public LuaBindingBuilder OneWay()
        {
            this.SetMode(BindingMode.OneWay);
            return this;
        }

        public LuaBindingBuilder OneWayToSource()
        {
            this.SetMode(BindingMode.OneWayToSource);
            return this;
        }

        public LuaBindingBuilder OneTime()
        {
            this.SetMode(BindingMode.OneTime);
            return this;
        }

        public LuaBindingBuilder CommandParameter(object parameter)
        {
            if (parameter is LuaFunction function)
                this.SetCommandParameter(function);
            else                
                this.SetCommandParameter(parameter);
            return this;
        }

        protected void SetCommandParameter(LuaFunction parameter)
        {
            this.description.CommandParameter = parameter;
            this.description.Converter = new ParameterWrapConverter(new LuaFunctionCommandParameter(parameter));
        }

        public LuaBindingBuilder WithConversion(string converterName)
        {
            var converter = this.ConverterByName(converterName);
            return this.WithConversion(converter);
        }

        public LuaBindingBuilder WithConversion(IConverter converter)
        {
            this.description.Converter = converter;
            return this;
        }

        public LuaBindingBuilder WithScopeKey(object scopeKey)
        {
            this.SetScopeKey(scopeKey);
            return this;
        }
    }
}