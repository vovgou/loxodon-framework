using UnityEngine;

namespace Loxodon.Framework.Views
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform), typeof(Canvas))]
    public abstract class GlobalWindowManagerBase : WindowManager
    {
    }
}