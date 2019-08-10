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

namespace Loxodon.Framework.Views
{
    public abstract class Transition : ITransition
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Transition));

        private Window window;
        private bool done = false;
        private bool animationDisabled = false;

        private bool running = false;

        //bind the StateChange event.
        private bool bound = false;
        private Action onStart;
        private Action<IWindow, WindowState> onStateChanged;
        private Action onFinish;

        public Transition(Window window)
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

        public virtual Window Window
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
            this.RaiseFinished();
            this.done = true;
            this.Unbind();
        }

        public ITransition DisableAnimation(bool disabled)
        {
            if (this.running)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The transition is running.DisableAnimation failed.");

                return this;
            }

            this.animationDisabled = disabled;
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

            this.onStateChanged += callback;
            return this;
        }

        public ITransition OnFinish(Action callback)
        {
            if (this.running)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The transition is running.OnFinish failed.");

                return this;
            }

            this.onFinish += callback;
            return this;
        }

        public virtual IEnumerator TransitionTask()
        {
            this.running = true;
            this.OnStart();
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
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
}
