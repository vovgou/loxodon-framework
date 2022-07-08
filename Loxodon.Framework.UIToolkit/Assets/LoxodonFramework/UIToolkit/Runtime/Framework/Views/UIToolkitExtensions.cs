using Loxodon.Framework.Views;
using UnityEngine.UIElements;

namespace Loxodon.Framework.Views
{
    public static class UIToolkitExtensions
    {
        public static T Q<T>(this UIToolkitWindow window, string name = null, params string[] classes) where T : VisualElement
        {
            return window.RootVisualElement.Q<T>(name, classes);
        }

        public static VisualElement Q(this UIToolkitWindow window, string name = null, params string[] classes)
        {
            return window.RootVisualElement.Q(name, classes);
        }

        public static T Q<T>(this UIToolkitWindow window, string name = null, string className = null) where T : VisualElement
        {
            return window.RootVisualElement.Q<T>(name, className);
        }

        public static VisualElement Q(this UIToolkitWindow window, string name = null, string className = null)
        {
            return window.RootVisualElement.Q(name, className);
        }

        public static UQueryBuilder<VisualElement> Query(this UIToolkitWindow window, string name = null, params string[] classes)
        {
            return window.RootVisualElement.Query(name, classes);
        }

        public static UQueryBuilder<VisualElement> Query(this UIToolkitWindow window, string name = null, string className = null)
        {
            return window.RootVisualElement.Query(name, className);
        }

        public static UQueryBuilder<T> Query<T>(this UIToolkitWindow window, string name = null, params string[] classes) where T : VisualElement
        {
            return window.RootVisualElement.Query<T>(name, classes);
        }

        public static UQueryBuilder<T> Query<T>(this UIToolkitWindow window, string name = null, string className = null) where T : VisualElement
        {
            return window.RootVisualElement.Query<T>(name, className);
        }

        public static UQueryBuilder<VisualElement> Query(this UIToolkitWindow window)
        {
            return window.RootVisualElement.Query();
        }
    }
}
