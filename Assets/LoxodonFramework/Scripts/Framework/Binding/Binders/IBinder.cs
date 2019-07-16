using Loxodon.Framework.Binding.Contexts;
using System.Collections.Generic;

namespace Loxodon.Framework.Binding.Binders
{
    public interface IBinder
    {
        IBinding Bind(IBindingContext bindingContext, object source, object target, BindingDescription bindingDescription);

        IEnumerable<IBinding> Bind(IBindingContext bindingContext, object source, object target, IEnumerable<BindingDescription> bindingDescriptions);

    }
}