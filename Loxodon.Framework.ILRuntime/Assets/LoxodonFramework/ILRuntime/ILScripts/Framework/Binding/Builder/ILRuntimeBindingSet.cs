//using Loxodon.Framework.Binding.Contexts;

//namespace Loxodon.Framework.Binding.Builder
//{
//    public class ILRuntimeBindingSet : BindingSetBase
//    {
//        private object target;
//        public ILRuntimeBindingSet(IBindingContext context, object target) : base(context)
//        {
//            this.target = target;
//        }

//        public virtual ILRuntimeBindingBuilder Bind()
//        {
//            var builder = new ILRuntimeBindingBuilder(this.context, this.target);
//            this.builders.Add(builder);
//            return builder;
//        }

//        public virtual ILRuntimeBindingBuilder Bind(object target)
//        {
//            var builder = new ILRuntimeBindingBuilder(context, target);
//            this.builders.Add(builder);
//            return builder;
//        }
//    }
//}
