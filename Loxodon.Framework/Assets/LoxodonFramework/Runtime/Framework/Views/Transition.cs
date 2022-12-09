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
using Loxodon.Log;
using Loxodon.Framework.Execution;
using Loxodon.Framework.Asynchronous;
using System.Runtime.CompilerServices;

namespace Loxodon.Framework.Views
{
    public abstract class Transition : ITransition
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Transition));

        private IManageable window;
        private bool done = false;
        private bool animationDisabled = false;
        private int layer = 0;
        private Func<IWindow, IWindow, ActionType> overlayPolicy;

        private bool running = false;

        //bind the StateChange event.
        private bool bound = false;
        private Action onStart;
        private Action<IWindow, WindowState> onStateChanged;
        private Action onFinish;

        public Transition(IManageable window)
        {
            this.window = window;
        }

        ~Transition()
        {
            this.Unbind();
        }

        protected virtual void Bind()
        {
            if (bound)
                return;

            this.bound = true;
            if (this.window != null)
                this.window.StateChanged += StateChanged;
        }

        protected virtual void Unbind()
        {
            if (!bound)
                return;

            this.bound = false;

            if (this.window != null)
                this.window.StateChanged -= StateChanged;
        }

        public virtual IManageable Window
        {
            get { return this.window; }
            set { this.window = value; }
        }

        public virtual bool IsDone
        {
            get { return this.done; }
            protected set { this.done = value; }
        }

        public virtual object WaitForDone()
        {
            return Executors.WaitWhile(() => !IsDone);
        }

        public virtual bool AnimationDisabled
        {
            get { return this.animationDisabled; }
            protected set { this.animationDisabled = value; }
        }

        public virtual int Layer
        {
            get { return this.layer; }
            protected set { this.layer = value; }
        }

        public virtual Func<IWindow, IWindow, ActionType> OverlayPolicy
        {
            get { return this.overlayPolicy; }
            protected set { this.overlayPolicy = value; }
        }

        protected void StateChanged(object sender, WindowStateEventArgs e)
        {
            this.RaiseStateChanged((IWindow)sender, e.State);
        }

        protected virtual void RaiseStart()
        {
            try
            {
                if (this.onStart != null)
                    this.onStart();
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.Warn("", e);
            }
        }

        protected virtual void RaiseStateChanged(IWindow window, WindowState state)
        {
            try
            {
                if (this.onStateChanged != null)
                    this.onStateChanged(window, state);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.Warn("", e);
            }
        }

        protected virtual void RaiseFinished()
        {
            try
            {
                if (this.onFinish != null)
                    this.onFinish();
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.Warn("", e);
            }
        }

        protected virtual void OnStart()
        {
            this.Bind();
            this.RaiseStart();
        }

        protected virtual void OnEnd()
        {
            this.done = true;
            this.RaiseFinished();
            this.Unbind();
        }

        public IAwaiter GetAwaiter()
        {
            return new TransitionAwaiter(this);
        }

        public ITransition DisableAnimation(bool disabled)
        {
            if (this.running)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The transition is running.DisableAnimation failed.");

                return this;
            }

            if (this.done)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The transition is done.DisableAnimation failed.");

                return this;
            }

            this.animationDisabled = disabled;
            return this;
        }

        public ITransition AtLayer(int layer)
        {
            if (this.running)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The transition is running.sets the layer failed.");

                return this;
            }

            if (this.done)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The transition is done.sets the layer failed.");

                return this;
            }

            this.layer = layer;
            return this;
        }

        public ITransition Overlay(Func<IWindow, IWindow, ActionType> policy)
        {
            if (this.running)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The transition is running.sets the policy failed.");

                return this;
            }

            if (this.done)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The transition is done.sets the policy failed.");

                return this;
            }

            this.OverlayPolicy = policy;
            return this;
        }

        public ITransition OnStart(Action callback)
        {
            if (this.running)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The transition is running.OnStart failed.");

                return this;
            }

            if (this.done)
            {
                callback();
                return this;
            }

            this.onStart += callback;
            return this;
        }

        public ITransition OnStateChanged(Action<IWindow, WindowState> callback)
        {
            if (this.running)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The transition is running.OnStateChanged failed.");

                return this;
            }

            if (this.done)
                return this;

            this.onStateChanged += callback;
            return this;
        }

        public ITransition OnFinish(Action callback)
        {
            if (this.done)
            {
                callback();
                return this;
            }

            this.onFinish += callback;
            return this;
        }

        public virtual IEnumerator TransitionTask()
        {
            this.running = true;
            this.OnStart();
#if UNITY_5_3_OR_NEWER
            yield return this.DoTransition();
#else
            var transitionAction = this.DoTransition();
            while (transitionAction.MoveNext())
                yield return transitionAction.Current;
#endif
            this.OnEnd();
        }

        protected abstract IEnumerator DoTransition();
    }

    public class CompletedTransition : Transition
    {
        public CompletedTransition(IManageable window) : base(window)
        {
            this.IsDone = true;
        }

        protected override IEnumerator DoTransition()
        {
            yield break;
        }
    }

    public struct TransitionAwaiter : IAwaiter, ICriticalNotifyCompletion
    {
        private Transition transition;

        public TransitionAwaiter(Transition transition)
        {
            this.transition = transition ?? throw new ArgumentNullException("transition");
        }

        public bool IsCompleted => transition.IsDone;

        public void GetResult()
        {
            if (!IsCompleted)
                throw new Exception("The task is not finished yet");
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");
            transition.OnFinish(continuation);
        }
    }
}
