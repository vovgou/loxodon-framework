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
using UnityEngine;

using Loxodon.Log;
using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Views
{
    [DisallowMultipleComponent]
    public abstract class Window : WindowView, IWindow
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Window));

        [SerializeField]
        private WindowType windowType = WindowType.FULL;
        private IWindowManager windowManager;

        private bool created = false;
        private bool dismissed = false;
        private bool activated = false;
        private ITransition dismissTransition;
        private WindowState state = WindowState.NONE;

        private readonly object _lock = new object();
        private EventHandler activatedChanged;
        private EventHandler visibilityChanged;
        private EventHandler onDismissed;
        private EventHandler<WindowStateEventArgs> stateChanged;

        public event EventHandler ActivatedChanged
        {
            add { lock (_lock) { this.activatedChanged += value; } }
            remove { lock (_lock) { this.activatedChanged -= value; } }
        }
        public event EventHandler VisibilityChanged
        {
            add { lock (_lock) { this.visibilityChanged += value; } }
            remove { lock (_lock) { this.visibilityChanged -= value; } }
        }
        public event EventHandler OnDismissed
        {
            add { lock (_lock) { this.onDismissed += value; } }
            remove { lock (_lock) { this.onDismissed -= value; } }
        }

        public event EventHandler<WindowStateEventArgs> StateChanged
        {
            add { lock (_lock) { this.stateChanged += value; } }
            remove { lock (_lock) { this.stateChanged -= value; } }
        }

        public IWindowManager WindowManager
        {
            get { return this.windowManager ?? (this.windowManager = GameObject.FindObjectOfType<GlobalWindowManagerBase>()); }
            set { this.windowManager = value; }
        }

        public bool Created { get { return this.created; } }

        public bool Dismissed { get { return this.dismissed; } }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.RaiseVisibilityChanged();
        }

        protected override void OnDisable()
        {
            this.RaiseVisibilityChanged();
            base.OnDisable();
        }

        public bool Activated
        {
            get { return this.activated; }
            protected set
            {
                if (this.activated == value)
                    return;

                this.activated = value;
                this.OnActivatedChanged();
                this.RaiseActivatedChanged();
            }
        }

        protected WindowState State
        {
            get { return this.state; }
            set
            {
                if (this.state.Equals(value))
                    return;

                this.state = value;
                this.RaiseStateChanged(this.state);
            }
        }

        public WindowType WindowType
        {
            get { return this.windowType; }
            set { this.windowType = value; }
        }

        protected void RaiseActivatedChanged()
        {
            try
            {
                if (this.activatedChanged != null)
                    this.activatedChanged(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.Warn("", e);
            }
        }

        protected void RaiseVisibilityChanged()
        {
            try
            {
                if (this.visibilityChanged != null)
                    this.visibilityChanged(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.Warn("", e);
            }
        }

        protected void RaiseOnDismissed()
        {
            try
            {
                if (this.onDismissed != null)
                    this.onDismissed(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.Warn("", e);
            }
        }

        protected void RaiseStateChanged(WindowState state)
        {
            try
            {
                if (this.stateChanged != null)
                    this.stateChanged(this, new WindowStateEventArgs(state));
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.Warn("", e);
            }
        }

        /// <summary>
        /// Activate
        /// </summary>
        /// <returns></returns>
        public virtual IAsyncTask Activate(bool ignoreAnimation)
        {
            return new AsyncTask((promise) =>
            {
                if(!this.Visibility)
                {
                    promise.SetException(new InvalidOperationException("The window is not visible."));
                    return;
                }

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
        public virtual IAsyncTask Passivate(bool ignoreAnimation)
        {
            return new AsyncTask((promise) =>
            {
                if (!this.Visibility)
                {
                    promise.SetException(new InvalidOperationException("The window is not visible."));
                    return;
                }

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

        protected virtual void OnActivatedChanged()
        {
            this.Interactable = this.Activated;
        }

        public void Create(IBundle bundle = null)
        {
            if (this.dismissTransition != null || this.dismissed)
                throw new ObjectDisposedException(this.Name);

            if (this.created)
                return;

            this.State = WindowState.CREATE_BEGIN;
            this.Visibility = false;
            this.Interactable = this.Activated;
            this.OnCreate(bundle);
            this.WindowManager.Add(this);
            this.created = true;
            this.State = WindowState.CREATE_END;
        }

        protected abstract void OnCreate(IBundle bundle);


        public ITransition Show(bool ignoreAnimation = false)
        {
            if (this.dismissTransition != null || this.dismissed)
                throw new InvalidOperationException("The window has been destroyed");

            if (this.Visibility)
                throw new InvalidOperationException("The window is already visible.");

            return this.WindowManager.Show(this).DisableAnimation(ignoreAnimation);
        }

        public virtual IAsyncTask DoShow(bool ignoreAnimation = false)
        {
            return new AsyncTask(promise =>
            {
                try
                {
                    if (!this.created)
                        this.Create();

                    this.OnShow();
                    this.Visibility = true;
                    this.State = WindowState.VISIBLE;
                    if (!ignoreAnimation && this.EnterAnimation != null)
                    {
                        this.EnterAnimation.OnStart(() =>
                        {
                            this.State = WindowState.ENTER_ANIMATION_BEGIN;
                        }).OnEnd(() =>
                        {
                            this.State = WindowState.ENTER_ANIMATION_END;
                            promise.SetResult();
                        }).Play();
                    }
                    else
                    {
                        promise.SetResult();
                    }
                }
                catch (Exception e)
                {
                    promise.SetException(e);

                    if (log.IsWarnEnabled)
                        log.WarnFormat("The window named \"{0}\" failed to open!Error:{1}", this.Name, e);
                }
            }, true).Start(30);
        }

        /// <summary>
        /// Called before the start of the display animation.
        /// </summary>
        protected virtual void OnShow()
        {
        }

        public ITransition Hide(bool ignoreAnimation = false)
        {
            if (!this.created)
                throw new InvalidOperationException("The window has not been created.");

            if (this.dismissed)
                throw new InvalidOperationException("The window has been destroyed.");

            if (!this.Visibility)
                throw new InvalidOperationException("The window is not visible.");

            return this.WindowManager.Hide(this).DisableAnimation(ignoreAnimation);
        }

        public virtual IAsyncTask DoHide(bool ignoreAnimation = false)
        {
            return new AsyncTask(promise =>
            {
                try
                {
                    if (!ignoreAnimation && this.ExitAnimation != null)
                    {
                        this.ExitAnimation.OnStart(() =>
                        {
                            this.State = WindowState.EXIT_ANIMATION_BEGIN;
                        }).OnEnd(() =>
                        {
                            this.State = WindowState.EXIT_ANIMATION_END;
                            this.Visibility = false;
                            this.State = WindowState.INVISIBLE;
                            this.OnHide();
                            promise.SetResult();
                        }).Play();
                    }
                    else
                    {
                        this.Visibility = false;
                        this.State = WindowState.INVISIBLE;
                        this.OnHide();
                        promise.SetResult();
                    }
                }
                catch (Exception e)
                {
                    promise.SetException(e);

                    if (log.IsWarnEnabled)
                        log.WarnFormat("The window named \"{0}\" failed to hide!Error:{1}", this.Name, e);
                }
            }, true).Start(30);
        }

        /// <summary>
        /// Called at the end of the hidden animation.
        /// </summary>
        protected virtual void OnHide()
        {
        }

        public ITransition Dismiss(bool ignoreAnimation = false)
        {
            if (this.dismissTransition != null)
                return this.dismissTransition;

            if (this.dismissed)
                throw new InvalidOperationException(string.Format("The window[{0}] has been destroyed.", this.Name));

            this.dismissTransition = this.WindowManager.Dismiss(this).DisableAnimation(ignoreAnimation);
            return this.dismissTransition;
        }

        public virtual void DoDismiss()
        {
            try
            {
                if (!this.dismissed)
                {
                    this.State = WindowState.DISSMISS_BEGIN;
                    this.dismissed = true;
                    this.OnDismiss();
                    this.RaiseOnDismissed();
                    this.WindowManager.Remove(this);

                    if (!this.IsDestroyed() && this.gameObject != null)
                        GameObject.Destroy(this.gameObject);
                    this.State = WindowState.DISMISS_END;
                    this.dismissTransition = null;
                }
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The window named \"{0}\" failed to dismiss!Error:{1}", this.Name, e);
            }
        }

        protected virtual void OnDismiss()
        {
        }

        protected override void OnDestroy()
        {
            if (!this.Dismissed && this.dismissTransition == null)
            {
                this.Dismiss(true);
            }
            base.OnDestroy();
        }
    }
}
