using UnityEngine;

using Loxodon.Framework.Views.Animations;

namespace Loxodon.Framework.Views
{
    /// <summary>
    /// UI view
    /// </summary>
    public interface IUIView : IView
    {
        RectTransform RectTransform { get; }

        float Alpha { get; set; }

        bool Interactable { get; set; }

        CanvasGroup CanvasGroup { get; }

        IAnimation EnterAnimation { get; set; }

        IAnimation ExitAnimation { get; set; }

    }
}
