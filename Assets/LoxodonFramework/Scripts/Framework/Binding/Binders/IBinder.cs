using System.Collections.Generic;

namespace Loxodon.Framework.Binding.Binders
{
    public interface IBinder
    {
        IBinding Bind(object source, object target, BindingDescription bindingDescription);

        IEnumerable<IBinding> Bind(object source, object target,IEnumerable<BindingDescription> bindingDescriptions);
       
    }
}