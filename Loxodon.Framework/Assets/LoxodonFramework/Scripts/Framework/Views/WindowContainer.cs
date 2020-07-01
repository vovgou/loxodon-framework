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

using System;
using System.Collections.Generic;
using UnityEngine;

using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Views
{
    [DisallowMultipleComponent]
    public class WindowContainer : Window, IWindowManager
    {
        public static WindowContainer Create(string name)
        {
            return Create(null, name);
        }

        public static WindowContainer Create(IWindowManager windowManager, string name)
        {
            GameObject root = new GameObject(name, typeof(CanvasGroup));
            RectTransform rectTransform = root.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.localPosition = Vector3.zero;

            WindowContainer container = root.AddComponent<WindowContainer>();
            container.WindowManager = windowManager;
            container.Create();
            container.Show(true);
            return container;
        }

        private IWindowManager localWindowManager;

        protected override void OnCreate(IBundle bundle)
        {
            /* Create Window View */
            this.WindowType = WindowType.FULL;

            this.localWindowManager = this.CreateWindowManager();
        }

        protected virtual IWindowManager CreateWindowManager()
        {
            return this.gameObject.AddComponent<WindowManager>();
        }

        protected override void OnActivatedChanged()
        {
            if (this.localWindowManager != null)
                this.localWindowManager.Activated = this.Activated;
            base.OnActivatedChanged();
        }

        bool IWindowManager.Activated
        {
            get { return localWindowManager.Activated; }
            set { localWindowManager.Activated = value; }
        }

        public IWindow Current { get { return localWindowManager.Current; } }

        public int Count { get { return localWindowManager.Count; } }

        public override IAsyncTask Activate(bool ignoreAnimation)
        {
            if (!this.Visibility)
                throw new InvalidOperationException("The window is not visible.");

            if (this.localWindowManager.Current != null)
            {
                this.Activated = true;
                return (this.localWindowManager.Current as Window).Activate(ignoreAnimation);
            }

            return new AsyncTask((promise) =>
            {
                if (this.Activated)
                {
                    promise.SetResult();
                    return;
                }

                if (!ignoreAnimation && this.ActivationAnimation != null)
                {
                    this.ActivationAnimation.OnStart(() =>
                    {
                        this.State = WindowState.ACTIVATION_ANIMATION_BEGIN;
                    }).OnEnd(() =>
                    {
                        this.State = WindowState.ACTIVATION_ANIMATION_END;
                        this.Activated = true;
                        this.State = WindowState.ACTIVATED;
                        promise.SetResult();
                    }).Play();
                }
                else
                {
                    this.Activated = true;
                    this.State = WindowState.ACTIVATED;
                    promise.SetResult();
                }
            }, true).Start(30);
        }

        /// <summary>
        /// Passivate
        /// </summary>
        /// <returns></returns>
        public override IAsyncTask Passivate(bool ignoreAnimation)
        {
            if (!this.Visibility)
                throw new InvalidOperationException("The window is not visible.");

            if (this.localWindowManager.Current != null)
            {
                return (this.localWindowManager.Current as Window).Passivate(ignoreAnimation).OnFinish(() =>
                {
                    this.Activated = false;
                });
            }

            return new AsyncTask((promise) =>
            {
                if (!this.Activated)
                {
                    promise.SetResult();
                    return;
                }

                this.Activated = false;
                this.State = WindowState.PASSIVATED;

                if (!ignoreAnimation && this.PassivationAnimation != null)
                {
                    this.PassivationAnimation.OnStart(() =>
                    {
                        this.State = WindowState.PASSIVATION_ANIMATION_BEGIN;
                    }).OnEnd(() =>
                    {
                        this.State = WindowState.PASSIVATION_ANIMATION_END;
                        promise.SetResult();
                    }).Play();
                }
                else
                {
                    promise.SetResult();
                }
            }, true).Start(30);
        }

        public IEnumerator<IWindow> Visibles()
        {
            return localWindowManager.Visibles();
        }

        public IWindow Get(int index)
        {
            return localWindowManager.Get(index);
        }

        public void Add(IWindow window)
        {
            localWindowManager.Add(window);
        }

        public bool Remove(IWindow window)
        {
            return localWindowManager.Remove(window);
        }

        public IWindow RemoveAt(int index)
        {
            return localWindowManager.RemoveAt(index);
        }

        public bool Contains(IWindow window)
        {
            return localWindowManager.Contains(window);
        }

        public int IndexOf(IWindow window)
        {
            return localWindowManager.IndexOf(window);
        }

        public List<IWindow> Find(bool visible)
        {
            return localWindowManager.Find(visible);
        }

        public T Find<T>() where T : IWindow
        {
            return localWindowManager.Find<T>();
        }

        public T Find<T>(string name) where T : IWindow
        {
            return localWindowManager.Find<T>(name);
        }

        public List<T> FindAll<T>() where T : IWindow
        {
            return localWindowManager.FindAll<T>();
        }

        public void Clear()
        {
            localWindowManager.Clear();
        }

        public ITransition Show(IWindow window)
        {
            return localWindowManager.Show(window);
        }

        public ITransition Hide(IWindow window)
        {
            return localWindowManager.Hide(window);
        }

        public ITransition Dismiss(IWindow window)
        {
            return localWindowManager.Dismiss(window);
        }
    }
}
