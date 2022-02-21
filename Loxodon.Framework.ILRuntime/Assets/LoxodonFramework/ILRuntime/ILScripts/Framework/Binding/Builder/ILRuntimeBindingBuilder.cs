//using Loxodon.Framework.Binding.Contexts;
//using Loxodon.Framework.Binding.Converters;
//using System;

//namespace Loxodon.Framework.Binding.Builder
//{
//    public class ILRuntimeBindingBuilder<TTarget, TSource> : BindingBuilderBase where TTarget : class
//    {
//        //private static readonly ILog log = LogManager.GetLogger(typeof(ILRuntimeBindingBuilder<TTarget, TSource>));

//        public ILRuntimeBindingBuilder(IBindingContext context, TTarget target) : base(context, target)
//        {
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> For(string targetName)
//        {
//            this.description.TargetName = targetName;
//            this.description.UpdateTrigger = null;
//            return this;
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> For(string targetName, string updateTrigger)
//        {
//            this.description.TargetName = targetName;
//            this.description.UpdateTrigger = updateTrigger;
//            return this;
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> To(string path)
//        {
//            this.SetMemberPath(path);
//            return this;
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> ToExpression<TResult>(Func<TSource, TResult> expression, params string[] paths)
//        {
//            //this.SetExpression(expression);
//            this.OneWay();
//            return this;
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> ToExpression<TResult>(Func<TResult> expression)
//        {
//            //this.SetExpression(expression);
//            this.OneWay();
//            return this;
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> ToStatic(string path)
//        {
//            this.SetStaticMemberPath(path);
//            return this;
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> ToValue(object value)
//        {
//            this.SetLiteral(value);
//            return this;
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> TwoWay()
//        {
//            this.SetMode(BindingMode.TwoWay);
//            return this;
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> OneWay()
//        {
//            this.SetMode(BindingMode.OneWay);
//            return this;
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> OneWayToSource()
//        {
//            this.SetMode(BindingMode.OneWayToSource);
//            return this;
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> OneTime()
//        {
//            this.SetMode(BindingMode.OneTime);
//            return this;
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> CommandParameter(object parameter)
//        {
//            this.SetCommandParameter(parameter);
//            return this;
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> WithConversion(string converterName)
//        {
//            var converter = this.ConverterByName(converterName);
//            return this.WithConversion(converter);
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> WithConversion(IConverter converter)
//        {
//            this.description.Converter = converter;
//            return this;
//        }

//        public ILRuntimeBindingBuilder<TTarget, TSource> WithScopeKey(object scopeKey)
//        {
//            this.SetScopeKey(scopeKey);
//            return this;
//        }
//    }

//    //    public class BindingBuilder<TTarget> : BindingBuilderBase where TTarget : class
//    //    {
//    //        //private static readonly ILog log = LogManager.GetLogger(typeof(BindingBuilder<TTarget>));

//    //        public BindingBuilder(IBindingContext context, TTarget target) : base(context, target)
//    //        {
//    //        }

//    //        public BindingBuilder<TTarget> For(string targetPropertyName)
//    //        {
//    //            this.description.TargetName = targetPropertyName;
//    //            this.description.UpdateTrigger = null;
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> For(string targetPropertyName, string updateTrigger)
//    //        {
//    //            this.description.TargetName = targetPropertyName;
//    //            this.description.UpdateTrigger = updateTrigger;
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> For<TResult>(Expression<Func<TTarget, TResult>> memberExpression)
//    //        {
//    //            string targetName = this.PathParser.ParseMemberName(memberExpression);
//    //            this.description.TargetName = targetName;
//    //            this.description.UpdateTrigger = null;
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> For<TResult>(Expression<Func<TTarget, TResult>> memberExpression, string updateTrigger)
//    //        {
//    //            string targetName = this.PathParser.ParseMemberName(memberExpression);
//    //            this.description.TargetName = targetName;
//    //            this.description.UpdateTrigger = updateTrigger;
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> For<TResult, TEvent>(Expression<Func<TTarget, TResult>> memberExpression, Expression<Func<TTarget, TEvent>> updateTriggerExpression)
//    //        {
//    //            string targetName = this.PathParser.ParseMemberName(memberExpression);
//    //            string updateTrigger = this.PathParser.ParseMemberName(updateTriggerExpression);
//    //            this.description.TargetName = targetName;
//    //            this.description.UpdateTrigger = updateTrigger;
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> For(Expression<Func<TTarget, EventHandler<InteractionEventArgs>>> memberExpression)
//    //        {
//    //            string targetName = this.PathParser.ParseMemberName(memberExpression);
//    //            this.description.TargetName = targetName;
//    //            this.description.UpdateTrigger = null;
//    //            this.OneWayToSource();
//    //            return this;
//    //        }

//    //#if UNITY_2019_1_OR_NEWER
//    //        public BindingBuilder<TTarget> For<TResult>(Expression<Func<TTarget, TResult>> memberExpression, Expression<Func<TTarget, Func<EventCallback<ChangeEvent<TResult>>, bool>>> updateTriggerExpression)
//    //        {
//    //            string targetName = this.PathParser.ParseMemberName(memberExpression);
//    //            string updateTrigger = this.PathParser.ParseMemberName(updateTriggerExpression);
//    //            this.description.TargetName = targetName;
//    //            this.description.UpdateTrigger = updateTrigger;
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> For<TResult>(Expression<Func<TTarget, Func<EventCallback<ChangeEvent<TResult>>, bool>>> memberExpression)
//    //        {
//    //            string targetName = this.PathParser.ParseMemberName(memberExpression);
//    //            this.description.TargetName = targetName;
//    //            this.description.UpdateTrigger = null;
//    //            this.OneWayToSource();
//    //            return this;
//    //        }
//    //#endif

//    //        public BindingBuilder<TTarget> To(string path)
//    //        {
//    //            this.SetStaticMemberPath(path);
//    //            this.OneWay();
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> To<TResult>(Expression<Func<TResult>> path)
//    //        {
//    //            this.SetStaticMemberPath(this.PathParser.ParseStaticPath(path));
//    //            this.OneWay();
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> To<TParameter>(Expression<Func<Action<TParameter>>> path)
//    //        {
//    //            this.SetStaticMemberPath(this.PathParser.ParseStaticPath(path));
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> To(Expression<Func<Action>> path)
//    //        {
//    //            this.SetStaticMemberPath(this.PathParser.ParseStaticPath(path));
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> ToValue(object value)
//    //        {
//    //            this.SetLiteral(value);
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> ToExpression<TResult>(Expression<Func<TResult>> expression)
//    //        {
//    //            this.SetExpression(expression);
//    //            this.OneWay();
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> TwoWay()
//    //        {
//    //            this.SetMode(BindingMode.TwoWay);
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> OneWay()
//    //        {
//    //            this.SetMode(BindingMode.OneWay);
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> OneWayToSource()
//    //        {
//    //            this.SetMode(BindingMode.OneWayToSource);
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> OneTime()
//    //        {
//    //            this.SetMode(BindingMode.OneTime);
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> CommandParameter(object parameter)
//    //        {
//    //            this.SetCommandParameter(parameter);
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> WithConversion(string converterName)
//    //        {
//    //            var converter = ConverterByName(converterName);
//    //            return this.WithConversion(converter);
//    //        }

//    //        public BindingBuilder<TTarget> WithConversion(IConverter converter)
//    //        {
//    //            this.description.Converter = converter;
//    //            return this;
//    //        }

//    //        public BindingBuilder<TTarget> WithScopeKey(object scopeKey)
//    //        {
//    //            this.SetScopeKey(scopeKey);
//    //            return this;
//    //        }
//    //    }

//    //public class BindingBuilder : BindingBuilderBase
//    //{
//    //    //private static readonly ILog log = LogManager.GetLogger(typeof(BindingBuilder));

//    //    public BindingBuilder(IBindingContext context, object target) : base(context, target)
//    //    {
//    //    }

//    //    public BindingBuilder For(string targetName, string updateTrigger = null)
//    //    {
//    //        this.description.TargetName = targetName;
//    //        this.description.UpdateTrigger = updateTrigger;
//    //        return this;
//    //    }

//    //    public BindingBuilder To(string path)
//    //    {
//    //        this.SetMemberPath(path);
//    //        return this;
//    //    }

//    //    public BindingBuilder ToStatic(string path)
//    //    {
//    //        this.SetStaticMemberPath(path);
//    //        return this;
//    //    }

//    //    public BindingBuilder ToValue(object value)
//    //    {
//    //        this.SetLiteral(value);
//    //        return this;
//    //    }

//    //    public BindingBuilder TwoWay()
//    //    {
//    //        this.SetMode(BindingMode.TwoWay);
//    //        return this;
//    //    }

//    //    public BindingBuilder OneWay()
//    //    {
//    //        this.SetMode(BindingMode.OneWay);
//    //        return this;
//    //    }

//    //    public BindingBuilder OneWayToSource()
//    //    {
//    //        this.SetMode(BindingMode.OneWayToSource);
//    //        return this;
//    //    }

//    //    public BindingBuilder OneTime()
//    //    {
//    //        this.SetMode(BindingMode.OneTime);
//    //        return this;
//    //    }

//    //    public BindingBuilder CommandParameter(object parameter)
//    //    {
//    //        this.SetCommandParameter(parameter);
//    //        return this;
//    //    }

//    //    public BindingBuilder WithConversion(string converterName)
//    //    {
//    //        var converter = this.ConverterByName(converterName);
//    //        return this.WithConversion(converter);
//    //    }

//    //    public BindingBuilder WithConversion(IConverter converter)
//    //    {
//    //        this.description.Converter = converter;
//    //        return this;
//    //    }

//    //    public BindingBuilder WithScopeKey(object scopeKey)
//    //    {
//    //        this.SetScopeKey(scopeKey);
//    //        return this;
//    //    }
//    //}
//}
