using Loxodon.Framework.Binding.Contexts;
using System.Collections.Generic;
using System.Linq;

namespace Loxodon.Framework.Binding.Binders
{
    public class StandardBinder : IBinder
    {
        protected IBindingFactory factory;

        public StandardBinder(IBindingFactory factory)
        {
            this.factory = factory;
        }

        public IBinding Bind(IBindingContext bindingContext, object source, object target, BindingDescription bindingDescription)
        {
            return factory.Create(bindingContext, source, target, bindingDescription);
        }

        public IEnumerable<IBinding> Bind(IBindingContext bindingContext, object source, object target, IEnumerable<BindingDescription> bindingDescriptions)
        {
            if (bindingDescriptions == null)
                return new IBinding[0];

            return bindingDescriptions.Select(description => this.Bind(bindingContext, source, target, description));
        }
    }
}