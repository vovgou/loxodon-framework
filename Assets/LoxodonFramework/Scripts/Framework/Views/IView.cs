using UnityEngine;

namespace Loxodon.Framework.Views
{
    /// <summary>
    /// View, it's anything that display layer.Such as scene , character , UI, etc
    /// </summary>
    public interface IView
    {
        string Name { get; set; }

        Transform Parent { get; }

        GameObject Owner { get; }

        Transform Transform { get; }

        bool Visibility { get; set; }

        /// <summary>
        /// External extended attributes
        /// </summary>
        IAttributes ExtraAttributes { get; }
    }
}

