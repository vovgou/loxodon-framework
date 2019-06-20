using System.Collections;
using UnityEngine;

using Loxodon.Framework.Contexts;
using System;

namespace Loxodon.Framework.Views
{
    public class Toast
    {
        private const string DEFAULT_VIEW_NAME = "UI/Toast";

        private static string viewName;
        public static string ViewName
        {
            get { return string.IsNullOrEmpty(viewName) ? DEFAULT_VIEW_NAME : viewName; }
            set { viewName = value; }
        }

        public static Toast Show(IUIViewGroup viewGroup, string text, float duration = 3f)
        {
            return Show(ViewName, viewGroup, text, duration, null, null);
        }

        public static Toast Show(IUIViewGroup viewGroup, string text, float duration, UILayout layout)
        {
            return Show(ViewName, viewGroup, text, duration, layout, null);
        }

        public static Toast Show(IUIViewGroup viewGroup, string text, float duration, UILayout layout, Action callback)
        {
            return Show(ViewName, viewGroup, text, duration, layout, callback);
        }

        public static Toast Show(string viewName, IUIViewGroup viewGroup, string text, float duration, UILayout layout, Action callback)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ViewName;

            ApplicationContext context = Context.GetApplicationContext();
            IUIViewLocator locator = context.GetService<IUIViewLocator>();
            ToastView view = locator.LoadView<ToastView>(viewName);
            if (view == null)
                throw new NotFoundException("Not found the \"ToastView\".");

            Toast toast = new Toast(view, viewGroup, text, duration, layout);
            toast.Show();
            return toast;
        }

        private readonly IUIViewGroup viewGroup;
        private readonly float duration;
        private readonly string text;
        private readonly ToastView view;
        private readonly UILayout layout;
        private readonly Action callback;

        protected Toast(ToastView view, IUIViewGroup viewGroup, string text, float duration) : this(view, viewGroup, text, duration, null, null)
        {
        }

        protected Toast(ToastView view, IUIViewGroup viewGroup, string text, float duration, UILayout layout) : this(view, viewGroup, text, duration, layout, null)
        {
        }

        protected Toast(ToastView view, IUIViewGroup viewGroup, string text, float duration, UILayout layout, Action callback)
        {
            this.view = view;
            this.viewGroup = viewGroup;
            this.text = text;
            this.duration = duration;
            this.layout = layout;
            this.callback = callback;
        }

        public float Duration
        {
            get { return this.duration; }
        }

        public string Text
        {
            get { return this.text; }
        }

        public ToastView View
        {
            get { return this.view; }
        }

        public void Cancel()
        {
            if (this.view == null || this.view.Owner == null)
                return;

            if (!this.view.Visibility)
            {
                GameObject.Destroy(this.view.Owner);
                return;
            }

            if (this.view.ExitAnimation != null)
            {
                this.view.ExitAnimation.OnEnd(() =>
                {
                    this.view.Visibility = false;
                    this.viewGroup.RemoveView(this.view);
                    GameObject.Destroy(this.view.Owner);
                    this.DoCallback();
                }).Play();
            }
            else
            {
                this.view.Visibility = false;
                this.viewGroup.RemoveView(this.view);
                GameObject.Destroy(this.view.Owner);
                this.DoCallback();
            }
        }

        public void Show()
        {
            if (this.view.Visibility)
                return;

            this.viewGroup.AddView(this.view, this.layout);
            this.view.Visibility = true;
            this.view.text.text = this.text;

            if (this.view.EnterAnimation != null)
                this.view.EnterAnimation.Play();

            this.view.StartCoroutine(DelayDismiss(duration));
        }

        protected IEnumerator DelayDismiss(float duration)
        {
            yield return new WaitForSeconds(duration);
            this.Cancel();
        }

        protected void DoCallback()
        {
            try
            {
                if (this.callback != null)
                    this.callback();
            }
            catch (Exception)
            {
            }
        }
    }
}
