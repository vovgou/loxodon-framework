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
using UnityEngine.EventSystems;

using Loxodon.Framework.Views.Animations;

namespace Loxodon.Framework.Views
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class UIView : UIBehaviour, IUIView
    {
        private IAnimation enterAnimation;
        private IAnimation exitAnimation;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        [NonSerialized]
        private IAttributes attributes = new Attributes();

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

        public virtual Transform Parent
        {
            get { return !this.IsDestroyed() && this.transform != null ? this.transform.parent : null; }
        }

        public virtual GameObject Owner
        {
            get { return this.IsDestroyed() ? null : this.gameObject; }
        }

        public virtual Transform Transform
        {
            get { return this.IsDestroyed() ? null : this.transform; }
        }

        public virtual RectTransform RectTransform
        {
            get
            {
                if (this.IsDestroyed())
                    return null;

                return this.rectTransform ?? (this.rectTransform = GetComponent<RectTransform>());
            }
        }

        public virtual bool Visibility
        {
            get { return !this.IsDestroyed() && this.gameObject != null ? this.gameObject.activeSelf : false; }
            set
            {
                if (this.IsDestroyed() || this.gameObject == null)
                    return;

                if (this.gameObject.activeSelf == value)
                    return;

                this.gameObject.SetActive(value);
            }
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

        protected override void OnEnable()
        {
            base.OnEnable();
            this.OnVisibilityChanged();
        }

        protected override void OnDisable()
        {
            this.OnVisibilityChanged();
            base.OnDisable();
        }

        public virtual float Alpha
        {
            get { return !this.IsDestroyed() && this.gameObject != null ? this.CanvasGroup.alpha : 0f; }
            set { if (!this.IsDestroyed() && this.gameObject != null) this.CanvasGroup.alpha = value; }
        }

        public virtual bool Interactable
        {
            get { return !this.IsDestroyed() && this.gameObject != null ? this.CanvasGroup.interactable : false; }
            set { if (!this.IsDestroyed() && this.gameObject != null) this.CanvasGroup.interactable = value; }
        }

        public virtual CanvasGroup CanvasGroup
        {
            get
            {
                if (this.IsDestroyed())
                    return null;

                return this.canvasGroup ?? (this.canvasGroup = GetComponent<CanvasGroup>());
            }
        }

        public virtual IAttributes ExtraAttributes { get { return this.attributes; } }        

        protected virtual void OnVisibilityChanged()
        {
        }
    }
}

