using System;

namespace Loxodon.Framework.Binding
{
    public interface IBinding : IDisposable
    {
        object Target { get; }

        object DataContext { get; set; }
    }
}
