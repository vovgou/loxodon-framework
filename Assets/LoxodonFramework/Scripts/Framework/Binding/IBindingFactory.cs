namespace Loxodon.Framework.Binding
{
    public interface IBindingFactory
    {
        IBinding Create(object source, object target, BindingDescription bindingDescription);
    }
}
