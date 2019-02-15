using UnityEngine;

namespace Loxodon.Framework.Views
{
    public delegate void Layout(Transform transform);

    /// <summary>
    /// View group
    /// </summary>
    public interface IViewGroup : IView
    {
        void AddView(IView view, bool worldPositionStays = false);

        void AddView(IView view, Layout layout);

        void RemoveView(IView view, bool worldPositionStays = false);
    }
}
