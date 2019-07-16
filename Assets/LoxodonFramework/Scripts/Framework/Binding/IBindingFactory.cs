using Loxodon.Framework.Binding.Contexts;

namespace Loxodon.Framework.Binding
{
    public interface IBindingFactory
    {
        IBinding Create(IBindingContext bindingContext, object source, object target, BindingDescription bindingDescription);
    }
}
