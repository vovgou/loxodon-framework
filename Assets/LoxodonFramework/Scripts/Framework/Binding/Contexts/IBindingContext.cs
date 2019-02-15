using System;
using System.Collections.Generic;

namespace Loxodon.Framework.Binding.Contexts
{
    public interface IBindingContext : IDisposable
    {
        event EventHandler DataContextChanged;

        object DataContext { get; set; }

        void Add(IBinding binding,object key=null);

        void Add(IEnumerable<IBinding> bindings,object key = null);

        void Add(object target, BindingDescription description,object key = null);

        void Add(object target, IEnumerable<BindingDescription> descriptions, object key = null);

        void Clear(object key);

        void Clear();
    }
}