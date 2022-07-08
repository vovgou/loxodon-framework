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

using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace Loxodon.Framework.Views.Animations
{
    public class UIToolkitAlphaAnimation : UIAnimation
    {
        [Range(0f, 1f)]
        public float from = 1f;
        [Range(0f, 1f)]
        public float to = 1f;

        public float duration = 2f;

        private UIToolkitWindow window;

        void OnEnable()
        {
            this.window = this.GetComponent<UIToolkitWindow>();
            switch (this.AnimationType)
            {
                case AnimationType.EnterAnimation:
                    this.window.EnterAnimation = this;
                    break;
                case AnimationType.ExitAnimation:
                    this.window.ExitAnimation = this;
                    break;
                case AnimationType.ActivationAnimation:
                    this.window.ActivationAnimation = this;
                    break;
                case AnimationType.PassivationAnimation:
                    this.window.PassivationAnimation = this;
                    break;
            }

            if (this.AnimationType == AnimationType.ActivationAnimation || this.AnimationType == AnimationType.EnterAnimation)
            {
                this.window.RootVisualElement.style.opacity = from;
            }
        }

        public override IAnimation Play()
        {
            this.OnStart();
            (this.window.RootVisualElement as ITransitionAnimations).Start(from, to, (int)(duration * 1000), (e, value) =>
            {
                this.window.RootVisualElement.style.opacity = value;
                if (value == to)
                    this.OnEnd();
            });
            return this;
        }
    }
}