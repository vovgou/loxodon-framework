using UnityEngine;

namespace Loxodon.Framework.Views
{
    public delegate void UILayout(RectTransform transform);

    public interface IUIViewGroup : IUIView
    {
        void AddView(IUIView view, bool worldPositionStays = false);

        void AddView(IUIView view, UILayout layout);

        void RemoveView(IUIView view, bool worldPositionStays = false);
    }
}
