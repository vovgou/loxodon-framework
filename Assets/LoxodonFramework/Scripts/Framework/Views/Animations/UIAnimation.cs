using System;
using UnityEngine;

namespace Loxodon.Framework.Views.Animations
{
    public enum AnimationType
    {
        EnterAnimation,
        ExitAnimation,
        ActivationAnimation,
        PassivationAnimation
    }

    public abstract class UIAnimation : MonoBehaviour, IAnimation
    {
        private Action _onStart;
        private Action _onEnd;

        [SerializeField]
        private AnimationType animationType;

        public AnimationType AnimationType
        {
            get { return this.animationType; }
            set { this.animationType = value; }
        }

        protected void OnStart()
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

        protected void OnEnd()
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

        public abstract IAnimation Play();
    }
}
