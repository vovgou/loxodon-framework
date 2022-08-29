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

using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Views.Animations;
using Loxodon.Log;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using IAsyncResult = Loxodon.Framework.Asynchronous.IAsyncResult;


namespace Loxodon.Framework.Views
{
    [RequireComponent(typeof(UIDocument))]
    [DisallowMultipleComponent]
    public abstract class UIToolkitWindow : MonoBehaviour, IWindow, IManageable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UIToolkitWindow));

        public static readonly IMessenger Messenger = Window.Messenger;

        [SerializeField]
        private WindowType windowType = WindowType.FULL;
        [SerializeField]
        [Range(0, 10)]
        private int windowPriority = 0;
        [SerializeField]
        private bool stateBroadcast = true;
        private IWindowManager windowManager;

        private bool initialized = false;
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

        private IAnimation enterAnimation;
        private IAnimation exitAnimation;
        private IAnimation activationAnimation;
        private IAnimation passivationAnimation;

        protected UIDocument document;
        public VisualElement RootVisualElement { get { return document != null ? document.rootVisualElement : null; } }

        public UIDocument Document { get { return this.document; } }

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

        public virtual IAnimation EnterAnimation
        {
            get { return this.enterAnimation; }
            set { this.enterAnimation = value; }
        }

        public virtual IAnimation ExitAnimation
        {
            get { return this.exitAnimation; }
            set { this.exitAnimation = value; }
        }

        public virtual IAnimation ActivationAnimation
        {
            get { return this.activationAnimation; }
            set { this.activationAnimation = value; }
        }

        public virtual IAnimation PassivationAnimation
        {
            get { return this.passivationAnimation; }
            set { this.passivationAnimation = value; }
        }

        public virtual string Name
        {
            get { return !this.IsDestroyed() && this.gameObject != null ? this.gameObject.name : null; }
            set
            {
                if (this.IsDestroyed() || this.gameObject == null)
                    return;

                this.gameObject.name = value;
            }
        }

        public virtual Transform Transform
        {
            get { return this.IsDestroyed() ? null : this.transform; }
        }

        public virtual bool Visibility
        {
            get
            {
                if (IsDestroyed() || !this.gameObject.activeSelf)
                    return false;

                VisualElement root = this.RootVisualElement;
                if (root == null || !root.visible)
                    return false;

                return true;
            }
            set
            {
                if (!IsDestroyed() && this.gameObject.activeSelf == false)
                    this.gameObject.SetActive(value);

                VisualElement root = this.RootVisualElement;
                if (root != null && root.visible != value)
                    root.visible = value;
            }
        }

        public IWindowManager WindowManager
        {
            get { return this.windowManager ?? (this.windowManager = GameObject.FindObjectOfType<GlobalWindowManagerBase>()); }
            set { this.windowManager = value; }
        }

        public bool Created { get { return this.created; } }

        public bool Dismissed { get { return this.dismissed; } }

        protected virtual bool Interactable
        {
            get
            {
                if (this.IsDestroyed() || this.gameObject == null)
                    return false;

                VisualElement root = this.RootVisualElement;
                return root != null ? root.enabledSelf : false;
            }
            set
            {
                if (this.IsDestroyed() || this.gameObject == null)
                    return;

                VisualElement root = this.RootVisualElement;
                if (root != null && root.enabledSelf != value)
                {
                    root.SetEnabled(value);
                    PanelEventHandler handler = GetPanelEventHandler();
                    PanelRaycaster raycaster = GetPanelRaycaster();
                    if (handler != null && raycaster != null && root.panel == handler.panel)
                        raycaster.enabled = value;
                }
            }
        }

        private PanelRaycaster GetPanelRaycaster()
        {
            if (EventSystem.current != null)
                return EventSystem.current.GetComponentInChildren<PanelRaycaster>();
            return null;
        }

        private PanelEventHandler GetPanelEventHandler()
        {
            if (EventSystem.current != null)
                return EventSystem.current.GetComponentInChildren<PanelEventHandler>();
            return null;
        }

        public virtual bool IsDestroyed()
        {
            return this == null;
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
            this.RaiseVisibilityChanged();
        }

        protected virtual void Awake()
        {
            Initialize();
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

                WindowState old = this.state;
                this.state = value;
                this.RaiseStateChanged(old, this.state);
            }
        }

        public WindowType WindowType
        {
            get { return this.windowType; }
            set { this.windowType = value; }
        }

        public int WindowPriority
        {
            get { return this.windowPriority; }
            set
            {
                if (value < 0)
                    this.windowPriority = 0;
                else if (value > 10)
                    this.windowPriority = 10;
                else
                    this.windowPriority = value;
            }
        }

        protected void RaiseActivatedChanged()
        {
            try
            {
                this.activatedChanged?.Invoke(this, EventArgs.Empty);
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
                this.visibilityChanged?.Invoke(this, EventArgs.Empty);
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
                this.onDismissed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.Warn("", e);
            }
        }
        protected void RaiseStateChanged(WindowState oldState, WindowState newState)
        {
            try
            {
                WindowStateEventArgs eventArgs = new WindowStateEventArgs(this, oldState, newState);
                if (GlobalSetting.enableWindowStateBroadcast && stateBroadcast)
                    Messenger.Publish(eventArgs);

                this.stateChanged?.Invoke(this, eventArgs);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
        }

        protected virtual void OnActivatedChanged()
        {
            this.Interactable = this.Activated;
        }

        protected virtual void Initialize()
        {
            if (!initialized)
            {
                this.document = GetComponent<UIDocument>();
                if (document.rootVisualElement == null)
                    document.visualTreeAsset = document.visualTreeAsset;

                initialized = true;
            }
        }

        public void Create(IBundle bundle = null)
        {
            if (this.dismissTransition != null || this.dismissed)
                throw new ObjectDisposedException(this.Name);

            if (this.created)
                return;

            Initialize();

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

        public ITransition Dismiss(bool ignoreAnimation = false)
        {
            if (this.dismissTransition != null)
                return this.dismissTransition;

            if (this.dismissed)
                throw new InvalidOperationException(string.Format("The window[{0}] has been destroyed.", this.Name));

            this.dismissTransition = this.WindowManager.Dismiss(this).DisableAnimation(ignoreAnimation);
            return this.dismissTransition;
        }

        public virtual IAsyncResult Activate(bool ignoreAnimation)
        {
            AsyncResult result = new AsyncResult();
            try
            {
                if (!this.Visibility)
                {
                    result.SetException(new InvalidOperationException("The window is not visible."));
                    return result;
                }

                if (this.Activated)
                {
                    result.SetResult();
                    return result;
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
                        result.SetResult();
                    }).Play();
                }
                else
                {
                    this.Activated = true;
                    this.State = WindowState.ACTIVATED;
                    result.SetResult();
                }
            }
            catch (Exception e)
            {
                result.SetException(e);
            }
            return result;
        }

        /// <summary>
        /// Passivate
        /// </summary>
        /// <returns></returns>
        public virtual IAsyncResult Passivate(bool ignoreAnimation)
        {
            AsyncResult result = new AsyncResult();
            try
            {
                if (!this.Visibility)
                {
                    result.SetException(new InvalidOperationException("The window is not visible."));
                    return result;
                }

                if (!this.Activated)
                {
                    result.SetResult();
                    return result;
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
                        result.SetResult();
                    }).Play();
                }
                else
                {
                    result.SetResult();
                }
            }
            catch (Exception e)
            {
                result.SetException(e);
            }
            return result;
        }

        public virtual IAsyncResult DoShow(bool ignoreAnimation = false)
        {
            AsyncResult result = new AsyncResult();
            Action<IPromise> action = promise =>
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
            };
            action(result);
            return result;
        }

        /// <summary>
        /// Called before the start of the display animation.
        /// </summary>
        protected virtual void OnShow()
        {
        }

        public virtual IAsyncResult DoHide(bool ignoreAnimation = false)
        {
            AsyncResult result = new AsyncResult();
            Action<IPromise> action = promise =>
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
            };
            action(result);
            return result;
        }

        /// <summary>
        /// Called at the end of the hidden animation.
        /// </summary>
        protected virtual void OnHide()
        {
        }

        public virtual void DoDismiss()
        {
            try
            {
                if (!this.dismissed)
                {
                    this.State = WindowState.DISMISS_BEGIN;
                    this.dismissed = true;
                    this.OnDismiss();
                    this.RaiseOnDismissed();
                    this.WindowManager.Remove(this);

                    if (!IsDestroyed())
#if UNITY_EDITOR
                        GameObject.DestroyImmediate(this.gameObject);
#else
                        GameObject.Destroy(this.gameObject);
#endif
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

        protected virtual void OnDestroy()
        {
            if (!this.Dismissed && this.dismissTransition == null)
            {
                this.Dismiss(true);
            }
        }
    }
}
