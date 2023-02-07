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
using Loxodon.Log;

namespace Loxodon.Framework.Views
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class UIView : UIBehaviour, IUIView
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UIView));

        private IAnimation enterAnimation;
        private IAnimation exitAnimation;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private readonly object _lock = new object();
        private EventHandler onDisabled;
        private EventHandler onEnabled;

        [NonSerialized]
        private IAttributes attributes = new Attributes();

        public event EventHandler OnDisabled
        {
            add { lock (_lock) { this.onDisabled += value; } }
            remove { lock (_lock) { this.onDisabled -= value; } }
        }

        public event EventHandler OnEnabled
        {
            add { lock (_lock) { this.onEnabled += value; } }
            remove { lock (_lock) { this.onEnabled -= value; } }
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
            this.RaiseOnEnabled();
        }

        protected override void OnDisable()
        {
            this.OnVisibilityChanged();
            base.OnDisable();
            this.RaiseOnDisabled();
        }

        protected void RaiseOnEnabled()
        {
            try
            {
                if (this.onEnabled != null)
                    this.onEnabled(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
        }

        protected void RaiseOnDisabled()
        {
            try
            {
                if (this.onDisabled != null)
                    this.onDisabled(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
        }

        public virtual float Alpha
        {
            get { return !this.IsDestroyed() && this.gameObject != null ? this.CanvasGroup.alpha : 0f; }
            set { if (!this.IsDestroyed() && this.gameObject != null) this.CanvasGroup.alpha = value; }
        }

        public virtual bool Interactable
        {
            get
            {
                if (this.IsDestroyed() || this.gameObject == null)
                    return false;

                if (GlobalSetting.useBlocksRaycastsInsteadOfInteractable)
                    return this.CanvasGroup.blocksRaycasts;
                return this.CanvasGroup.interactable;
            }
            set
            {
                if (this.IsDestroyed() || this.gameObject == null)
                    return;

                if (GlobalSetting.useBlocksRaycastsInsteadOfInteractable)
                    this.CanvasGroup.blocksRaycasts = value;
                else
                    this.CanvasGroup.interactable = value;
            }
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

