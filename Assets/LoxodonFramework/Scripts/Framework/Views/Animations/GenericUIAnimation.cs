using System;

namespace Loxodon.Framework.Views.Animations
{
    public delegate void AnimationAction<T>(T view, Action startCallback, Action endCallback) where T : IUIView;

    public class GenericUIAnimation<T> : IAnimation where T : IUIView
    {
        private T view;
        private AnimationAction<T> animation;

        private Action _onStart;
        private Action _onEnd;

        public GenericUIAnimation(T view, AnimationAction<T> animation)
        {
            this.view = view;
            this.animation = animation;
        }

        protected virtual void OnStart()
        {
            try
            {
                if (this._onStart != null)
                {
                    this._onStart();
                    this._onStart = null;
                }
            }
            catch (Exception) { }
        }

        protected virtual void OnEnd()
        {
            try
            {
                if (this._onEnd != null)
                {
                    this._onEnd();
                    this._onEnd = null;
                }
            }
            catch (Exception) { }
        }

        public IAnimation OnStart(Action onStart)
        {
            this._onStart += onStart;
            return this;
        }

        public IAnimation OnEnd(Action onEnd)
        {
            this._onEnd += onEnd;
            return this;
        }

        public virtual IAnimation Play()
        {
            if (this.animation != null)
                this.animation(this.view, OnStart, OnEnd);
            return this;
        }
    }
}
