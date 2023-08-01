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

using Loxodon.Framework.Execution;
using IAsyncResult = Loxodon.Framework.Asynchronous.IAsyncResult;

namespace Loxodon.Framework.Views
{
    [DisallowMultipleComponent]
    public class WindowManager : MonoBehaviour, IWindowManager
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(WindowManager));
        private static BlockingCoroutineTransitionExecutor blockingExecutor;

        //For compatibility with the "Configurable Enter Play Mode" feature
#if UNITY_2019_3_OR_NEWER //&& UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void OnInitialize()
        {
            if (blockingExecutor != null)
                blockingExecutor = null;
        }
#endif
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

        public int VisibleCount { get { return this.windows.FindAll(w => w.Visibility).Count; } }

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

        public virtual IWindow GetVisibleWindow(int index)
        {
            if (this.windows == null || this.windows.Count <= 1)
                return null;

            int currIndex = -1;
            var ie = this.Visibles();
            while (ie.MoveNext())
            {
                currIndex++;
                if (currIndex > index)
                    return null;

                if (currIndex == index)
                    return ie.Current;
            }
            return null;
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
            this.AddChild(GetTransform(window));
        }

        public virtual bool Remove(IWindow window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            this.RemoveChild(GetTransform(window));
            return this.windows.Remove(window);
        }

        public virtual IWindow RemoveAt(int index)
        {
            if (index < 0 || index > this.windows.Count - 1)
                throw new IndexOutOfRangeException();

            var window = this.windows[index];
            this.RemoveChild(GetTransform(window));
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
                Transform transform = GetTransform(window);
                if (transform != null)
                    transform.SetAsFirstSibling();
            }
        }

        protected virtual void MoveToFirst(IWindow window)
        {
            this.MoveToIndex(window, 0);
        }

        protected virtual void MoveToIndex(IWindow window, int index)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            int oldIndex = this.IndexOf(window);
            try
            {
                if (oldIndex < 0 || oldIndex == index)
                    return;

                this.windows.RemoveAt(oldIndex);
                this.windows.Insert(index, window);
            }
            finally
            {
                Transform transform = GetTransform(window);
                if (transform != null)
                {
                    if (index == 0)
                    {
                        transform.SetAsLastSibling();
                    }
                    else
                    {
                        IWindow preWindow = this.windows[index - 1];
                        int preWindowPosition = GetChildIndex(GetTransform(preWindow));
                        int currWindowPosition = oldIndex >= index ? preWindowPosition - 1 : preWindowPosition;
                        transform.SetSiblingIndex(currWindowPosition);
                    }
                }
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

        protected virtual Transform GetTransform(IWindow window)
        {
            try
            {
                if (window == null)
                    return null;

                if (window is UIView)
                    return (window as UIView).Transform;

                var propertyInfo = window.GetType().GetProperty("Transform");
                if (propertyInfo != null)
                    return (Transform)propertyInfo.GetGetMethod().Invoke(window, null);

                if (window is Component)
                    return (window as Component).transform;
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected virtual int GetChildIndex(Transform child)
        {
            Transform transform = this.transform;
            int count = transform.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                if (transform.GetChild(i).Equals(child))
                    return i;
            }
            return -1;
        }

        protected virtual void AddChild(Transform child, bool worldPositionStays = false)
        {
            if (child == null || this.transform.Equals(child.parent))
                return;

            child.gameObject.layer = this.gameObject.layer;
            child.SetParent(this.transform, worldPositionStays);
            child.SetAsFirstSibling();
        }

        protected virtual void RemoveChild(Transform child, bool worldPositionStays = false)
        {
            if (child == null || !this.transform.Equals(child.parent))
                return;

            child.SetParent(null, worldPositionStays);
        }

        public ITransition Show(IWindow window)
        {
            ShowTransition transition = new ShowTransition(this, (IManageable)window);
            GetTransitionExecutor().Execute(transition);
            return transition.OnStateChanged((w, state) =>
                {
                    /* Control the layer of the window */
                    if (state == WindowState.VISIBLE)
                        this.MoveToIndex(w, transition.Layer);

                    //if (state == WindowState.INVISIBLE)
                    //    this.MoveToLast(w);
                });
        }

        public ITransition Hide(IWindow window)
        {
            HideTransition transition = new HideTransition(this, (IManageable)window, false);
            GetTransitionExecutor().Execute(transition);
            return transition.OnStateChanged((w, state) =>
                {
                    /* Control the layer of the window */
                    if (state == WindowState.INVISIBLE)
                        this.MoveToLast(w);
                });
        }

        public ITransition Dismiss(IWindow window)
        {
            HideTransition transition = new HideTransition(this, (IManageable)window, true);
            GetTransitionExecutor().Execute(transition);
            return transition.OnStateChanged((w, state) =>
                {
                    /* Control the layer of the window */
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

        class ShowTransition : Transition
        {
            private WindowManager manager;
            public ShowTransition(WindowManager manager, IManageable window) : base(window)
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
                IManageable current = this.Window;
                int layer = (this.Layer < 0 || current.WindowType == WindowType.DIALOG || current.WindowType == WindowType.PROGRESS) ? 0 : this.Layer;
                if (layer > 0)
                {
                    int visibleCount = this.manager.VisibleCount;
                    if (layer > visibleCount)
                        layer = visibleCount;
                }
                this.Layer = layer;

                IManageable previous = (IManageable)this.manager.GetVisibleWindow(layer);
                if (previous != null)
                {
                    //Passivate the previous window
                    if (previous.Activated)
                    {
                        IAsyncResult passivate = previous.Passivate(this.AnimationDisabled);
                        yield return passivate.WaitForDone();
                    }

                    Func<IWindow, IWindow, ActionType> policy = this.OverlayPolicy;
                    if (policy == null)
                        policy = this.Overlay;
                    ActionType actionType = policy(previous, current);
                    switch (actionType)
                    {
                        case ActionType.Hide:
                            previous.DoHide(this.AnimationDisabled);
                            break;
                        case ActionType.Dismiss:
                            previous.DoHide(this.AnimationDisabled).Callbackable().OnCallback((r) =>
                            {
                                previous.DoDismiss();
                            });
                            break;
                        default:
                            break;
                    }
                }

                if (!current.Visibility)
                {
                    IAsyncResult show = current.DoShow(this.AnimationDisabled);
                    yield return show.WaitForDone();
                }

                if (this.manager.Activated && current.Equals(this.manager.Current))
                {
                    IAsyncResult activate = current.Activate(this.AnimationDisabled);
                    yield return activate.WaitForDone();
                }
            }
        }

        class HideTransition : Transition
        {
            private WindowManager manager;
            private bool dismiss;
            public HideTransition(WindowManager manager, IManageable window, bool dismiss) : base(window)
            {
                this.dismiss = dismiss;
                this.manager = manager;
            }

            protected override IEnumerator DoTransition()
            {
                IManageable current = this.Window;
                if (this.manager.IndexOf(current) == 0 && current.Activated)
                {
                    IAsyncResult passivate = current.Passivate(this.AnimationDisabled);
                    yield return passivate.WaitForDone();
                }

                if (current.Visibility)
                {
                    IAsyncResult hide = current.DoHide(this.AnimationDisabled);
                    yield return hide.WaitForDone();
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
                try
                {
                    if (transition is ShowTransition && transition.Window.WindowType == WindowType.QUEUED_POPUP)
                    {
                        int index = this.transitions.FindLastIndex((t) => (t is ShowTransition)
                        && t.Window.WindowType == WindowType.QUEUED_POPUP
                        && t.Window.WindowManager == transition.Window.WindowManager
                        && t.Window.WindowPriority >= transition.Window.WindowPriority);
                        if (index >= 0)
                        {
                            this.transitions.Insert(index + 1, transition);
                            return;
                        }

                        index = this.transitions.FindIndex((t) => (t is ShowTransition)
                        && t.Window.WindowType == WindowType.QUEUED_POPUP
                        && t.Window.WindowManager == transition.Window.WindowManager
                        && t.Window.WindowPriority < transition.Window.WindowPriority);
                        if (index >= 0)
                        {
                            this.transitions.Insert(index, transition);
                            return;
                        }
                    }

                    this.transitions.Add(transition);
                }
                finally
                {
                    if (!this.running)
                        taskResult = Executors.RunOnCoroutine(this.DoTask());
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
                if (!(transition is ShowTransition))
                    return true;

                IManageable window = transition.Window;
                IWindowManager manager = window.WindowManager;
                var current = manager.Current;
                if (current == null)
                    return true;

                if (current.WindowType == WindowType.DIALOG || current.WindowType == WindowType.PROGRESS)
                    return false;

                if (current.WindowType == WindowType.QUEUED_POPUP && !(window.WindowType == WindowType.DIALOG || window.WindowType == WindowType.PROGRESS))
                    return false;
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
                                IAsyncResult activate = (current as IManageable).Activate(transition.AnimationDisabled);
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
