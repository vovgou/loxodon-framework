using Loxodon.Framework.Binding.Contexts;
using System;

namespace Loxodon.Framework.Binding
{
    public interface IBinding : IDisposable
    {
        IBindingContext BindingContext { get; set; }

        object Target { get; }

        object DataContext { get; set; }
    }
}
