/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Contexts;
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
