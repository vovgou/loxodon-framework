using System.Collections.Generic;
using System.Linq;

using Loxodon.Log;

namespace Loxodon.Framework.Binding.Binders
{   
    public class StandardBinder : IBinder
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(StandardBinder));

        protected IBindingFactory factory;

        public StandardBinder(IBindingFactory factory)
        {
            this.factory  = factory;
        }
        
        public IBinding Bind(object source, object target, BindingDescription bindingDescription)
        {
            return factory.Create(source, target, bindingDescription);
        }

        public IEnumerable<IBinding> Bind(object source, object target,IEnumerable<BindingDescription> bindingDescriptions)
        {
            if (bindingDescriptions == null)
                return new IBinding[0];

            return bindingDescriptions.Select(description => this.Bind(source, target, description));
        }
    }
}