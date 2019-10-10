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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if NETFX_CORE
using System.Reflection;
#endif

using Loxodon.Log;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Views
{
    [DisallowMultipleComponent]
    public class WindowManager : MonoBehaviour, IWindowManager
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(WindowManager));
        private static BlockingCoroutineTransitionExecutor blockingExecutor;
        private static BlockingCoroutineTransitionExecutor GetTransitionExecutor()
        {
            if (blockingExecutor == null)
                blockingExecutor = new BlockingCoroutineTransitionExecutor();
            return blockingExecutor;
        }

        private bool lastActivated = true;
        private bool activated = true;
        private List<IWindow> windows = new List<IWindow>();

        public virtual bool Activated
        {
            get { return this.activated; }
            set
            {
                if (this.activated == value)
                    return;

                this.activated = value;
            }
        }

        public int Count { get { return this.windows.Count; } }

        public virtual IWindow Current
        {
            get
            {
                if (this.windows == null || this.windows.Count <= 0)
                    return null;

                IWindow window = this.windows[0];
                return window != null && window.Visibility ? window : null;
            }
        }

        public virtual IWindow Second
        {
            get
            {
                if (this.windows == null || this.windows.Count <= 1)
                    return null;

                IWindow window = this.windows[1];
                return window != null && window.Visibility ? window : null;
            }
        }

        protected virtual void OnEnable()
        {
            this.Activated = this.lastActivated;
        }

        protected virtual void OnDisable()
        {
            this.lastActivated = this.Activated;
            this.Activated = false;
        }

        protected virtual void OnDestroy()
        {
            if (this.windows.Count > 0)
            {
                this.Clear();
            }
        }

        protected virtual void OnApplicationQuit()
        {
            if (blockingExecutor != null)
            {
                blockingExecutor.Shutdown();
                blockingExecutor = null;
            }
        }

        public virtual void Clear()
        {
            for (int i = 0; i < this.windows.Count; i++)
            {
                try
                {
                    this.windows[i].Dismiss(true);
                }
                catch (Exception) { }
            }
            this.windows.Clear();
        }

        public virtual bool Contains(IWindow window)
        {
            return this.windows.Contains(window);
        }

        public virtual int IndexOf(IWindow window)
        {
            return this.windows.IndexOf(window);
        }

        public virtual IWindow Get(int index)
        {
            if (index < 0 || index > this.windows.Count - 1)
                throw new IndexOutOfRangeException();

            return this.windows[index];
        }

        public virtual void Add(IWindow window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            if (this.windows.Contains(window))
                return;

            this.windows.Add(window);
            this.AddView(window as UIView);
        }

        public virtual bool Remove(IWindow window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            this.RemoveView(window as UIView);
            return this.windows.Remove(window);
        }

        public virtual IWindow RemoveAt(int index)
        {
            if (index < 0 || index > this.windows.Count - 1)
                throw new IndexOutOfRangeException();

            var window = this.windows[index];
            this.RemoveView(window as UIView);
            this.windows.RemoveAt(index);
            return window;
        }

        protected virtual void MoveToLast(IWindow window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            try
            {
                int index = this.IndexOf(window);
                if (index < 0 || index == this.Count - 1)
                    return;

                this.windows.RemoveAt(index);
                this.windows.Add(window);
            }
            finally
            {
                var view = window as UIView;
                if (view != null && view.Transform != null)
                    view.Transform.SetAsFirstSibling();
            }
        }

        protected virtual void MoveToFirst(IWindow window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            try
            {
                int index = this.IndexOf(window);
                if (index < 0 || index == 0)
                    return;

                this.windows.RemoveAt(index);
                this.windows.Insert(0, window);
            }
            finally
            {
                var view = window as UIView;
                if (view != null && view.Transform != null)
                    view.Transform.SetAsLastSibling();
            }
        }

        public virtual IEnumerator<IWindow> Visibles()
        {
            return new InternalVisibleEnumerator(this.windows);
        }

        public virtual List<IWindow> Find(bool visible)
        {
            return this.windows.FindAll(w => w.Visibility == visible);
        }

        public virtual IWindow Find(Type windowType)
        {
            if (windowType == null)
                return null;

            return this.windows.Find(w => windowType.IsAssignableFrom(w.GetType()));
        }

        public virtual T Find<T>() where T : IWindow
        {
            return (T)this.windows.Find(w => w is T);
        }

        public virtual IWindow Find(string name, Type windowType)
        {
            if (name == null || windowType == null)
                return null;

            return this.windows.Find(w => windowType.IsAssignableFrom(w.GetType()) && w.Name.Equals(name));
        }

        public virtual T Find<T>(string name) where T : IWindow
        {
            return (T)this.windows.Find(w => w is T && w.Name.Equals(name));
        }

        public virtual List<IWindow> FindAll(Type windowType)
        {
            List<IWindow> list = new List<IWindow>();
            foreach (IWindow window in this.windows)
            {
                if (windowType.IsAssignableFrom(window.GetType()))
                    list.Add(window);
            }
            return list;
        }

        public virtual List<T> FindAll<T>() where T : IWindow
        {
            List<T> list = new List<T>();
            foreach (IWindow window in this.windows)
            {
                if (window is T)
                    list.Add((T)window);
            }
            return list;
        }

        protected virtual void AddView(IUIView view, bool worldPositionStays = false)
        {
            if (view == null)
                return;

            Transform t = view.Transform;
            if (t == null || this.transform.Equals(t.parent))
                return;

            view.Owner.layer = this.gameObject.layer;
            t.SetParent(this.transform, worldPositionStays);
            t.SetAsFirstSibling();
        }

        protected virtual void RemoveView(IUIView view, bool worldPositionStays = false)
        {
            if (view == null)
                return;

            Transform t = view.Transform;
            if (t == null || !this.transform.Equals(t.parent))
                return;

            t.SetParent(null, worldPositionStays);
        }

        public ITransition Show(IWindow window)
        {
            ShowTransition transition = new ShowTransition(this, (Window)window);
            GetTransitionExecutor().Execute(transition);
            return transition.OnStateChanged((w, state) =>
                {
                    /* Control the layer of the window */
                    if (state == WindowState.VISIBLE)
                        this.MoveToFirst(w);

                    if (state == WindowState.INVISIBLE)
                        this.MoveToLast(w);
                });
        }

        public ITransition Hide(IWindow window)
        {
            HideTransition transition = new HideTransition(this, (Window)window, false);
            GetTransitionExecutor().Execute(transition);
            return transition.OnStateChanged((w, state) =>
                {
                    /* Control the layer of the window */
                    if (state == WindowState.VISIBLE)
                        this.MoveToFirst(w);

                    if (state == WindowState.INVISIBLE)
                        this.MoveToLast(w);
                });
        }

        public ITransition Dismiss(IWindow window)
        {
            HideTransition transition = new HideTransition(this, (Window)window, true);
            GetTransitionExecutor().Execute(transition);
            return transition.OnStateChanged((w, state) =>
                {
                    /* Control the layer of the window */
                    if (state == WindowState.VISIBLE)
                        this.MoveToFirst(w);

                    if (state == WindowState.INVISIBLE)
                        this.MoveToLast(w);
                });
        }

        class InternalVisibleEnumerator : IEnumerator<IWindow>
        {
            private List<IWindow> windows;
            private int index = -1;
            public InternalVisibleEnumerator(List<IWindow> list)
            {
                this.windows = list;
            }

            public IWindow Current
            {
                get { return this.index < 0 || this.index >= this.windows.Count ? null : this.windows[index]; }
            }

            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            public void Dispose()
            {
                this.index = -1;
                this.windows.Clear();
            }

            public bool MoveNext()
            {
                if (index >= this.windows.Count - 1)
                    return false;

                index++;
                for (; index < this.windows.Count; index++)
                {
                    IWindow window = this.windows[index];
                    if (window != null && window.Visibility)
                        return true;
                }

                return false;
            }

            public void Reset()
            {
                this.index = -1;
            }
        }

        public enum ActionType
        {
            None,
            Hide,
            Dismiss
        }

        class ShowTransition : Transition
        {
            private WindowManager manager;
            public ShowTransition(WindowManager manager, Window window) : base(window)
            {
                this.manager = manager;
            }

            protected virtual ActionType Overlay(IWindow previous, IWindow current)
            {
                if (previous == null || previous.WindowType == WindowType.FULL)
                    return ActionType.None;

                if (previous.WindowType == WindowType.POPUP)
                    return ActionType.Dismiss;

                return ActionType.None;
            }

            protected override IEnumerator DoTransition()
            {
                Window current = this.Window;
                Window previous = this.manager.Current as Window;
                if (previous != null)
                {
                    //Passivate the previous window
                    if (previous.Activated)
                    {
                        IAsyncTask passivate = previous.Passivate(this.AnimationDisabled).Start();
                        yield return passivate.WaitForDone();
                    }

                    ActionType actionType = this.Overlay(previous, current);
                    switch (actionType)
                    {
                        case ActionType.Hide:
                            previous.DoHide(this.AnimationDisabled).Start();
                            break;
                        case ActionType.Dismiss:
                            previous.DoHide(this.AnimationDisabled).OnFinish(() =>
                            {
                                previous.DoDismiss();
                            }).Start();
                            break;
                        default:
                            break;
                    }
                }

                if (!current.Visibility)
                {
                    IAsyncTask show = current.DoShow(this.AnimationDisabled).Start();
                    yield return show.WaitForDone();
                }

                if (this.manager.Activated && current.Equals(this.manager.Current))
                {
                    IAsyncTask activate = current.Activate(this.AnimationDisabled).Start();
                    yield return activate.WaitForDone();
                }
            }
        }

        class HideTransition : Transition
        {
            private WindowManager manager;
            private bool dismiss;
            public HideTransition(WindowManager manager, Window window, bool dismiss) : base(window)
            {
                this.dismiss = dismiss;
                this.manager = manager;
            }

            protected override IEnumerator DoTransition()
            {
                Window current = this.Window;
                if (this.manager.IndexOf(current) == 0)
                {
                    if (current.Activated)
                    {
                        IAsyncTask passivate = current.Passivate(this.AnimationDisabled).Start();
                        yield return passivate.WaitForDone();
                    }

                    if (current.Visibility)
                    {
                        IAsyncTask hide = current.DoHide(this.AnimationDisabled).Start();
                        yield return hide.WaitForDone();
                    }
                }
                else
                {
                    if (current.Visibility)
                    {
                        IAsyncTask hide = current.DoHide(this.AnimationDisabled).Start();
                        yield return hide.WaitForDone();
                    }
                }

                if (dismiss)
                {
                    current.DoDismiss();
                }
            }
        }

        class BlockingCoroutineTransitionExecutor
        {
            private Asynchronous.IAsyncResult taskResult;
            private bool running = false;
            private List<Transition> transitions = new List<Transition>();
            public bool IsRunning { get { return this.running; } }

            public int Count { get { return this.transitions.Count; } }

            public BlockingCoroutineTransitionExecutor()
            {
            }

            public void Execute(Transition transition)
            {
                this.transitions.Add(transition);
                if (!this.running)
                {
                    this.taskResult = Executors.RunOnCoroutine(this.DoTask());
                }
            }

            public void Shutdown()
            {
                if (this.taskResult != null)
                {
                    this.taskResult.Cancel();
                    this.running = false;
                    this.taskResult = null;
                }
                this.transitions.Clear();
            }

            private bool Check(Transition transition)
            {
                if (transition is ShowTransition)
                {
                    IWindowManager manager = transition.Window.WindowManager;
                    var current = manager.Current;
                    if (current != null && (current.WindowType == WindowType.DIALOG || current.WindowType == WindowType.PROGRESS))
                        return false;
                }
                return true;
            }

            protected virtual IEnumerator DoTask()
            {
                try
                {
                    this.running = true;
                    yield return null;//wait one frame
                    while (this.transitions.Count > 0)
                    {
                        Transition transition = this.transitions.Find(e => Check(e));
                        if (transition != null)
                        {
                            this.transitions.Remove(transition);
                            var result = Executors.RunOnCoroutine(transition.TransitionTask());
                            yield return result.WaitForDone();

                            IWindowManager manager = transition.Window.WindowManager;
                            var current = manager.Current;
                            if (manager.Activated && current != null && !current.Activated && !this.transitions.Exists((e) => e.Window.WindowManager.Equals(manager)))
                            {
                                IAsyncTask activate = (current as Window).Activate(transition.AnimationDisabled).Start();
                                yield return activate.WaitForDone();
                            }
                        }
                        else
                        {
                            yield return null;
                        }
                    }
                }
                finally
                {
                    this.running = false;
                    this.taskResult = null;
                }
            }
        }
    }
}
