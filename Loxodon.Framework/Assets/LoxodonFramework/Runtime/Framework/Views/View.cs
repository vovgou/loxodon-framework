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

namespace Loxodon.Framework.Views
{
    public class View : MonoBehaviour, IView
    {
        [NonSerialized]
        private IAttributes attributes = new Attributes();

        public virtual string Name
        {
            get { return this.gameObject != null ? this.gameObject.name : null; }
            set
            {
                if (this.gameObject == null)
                    return;

                this.gameObject.name = value;
            }
        }

        public virtual Transform Parent
        {
            get { return this.transform != null ? this.transform.parent : null; }
        }

        public virtual GameObject Owner
        {
            get { return this.gameObject; }
        }

        public virtual Transform Transform
        {
            get { return this.transform; }
        }

        public virtual bool Visibility
        {
            get { return this.gameObject != null ? this.gameObject.activeSelf : false; }
            set
            {
                if (this.gameObject == null)
                    return;
                if (this.gameObject.activeSelf == value)
                    return;

                this.gameObject.SetActive(value);
            }
        }

        protected virtual void OnEnable()
        {
            this.OnVisibilityChanged();
        }

        protected virtual void OnDisable()
        {
            this.OnVisibilityChanged();
        }

        public virtual IAttributes ExtraAttributes { get { return this.attributes; } }       

        protected virtual void OnVisibilityChanged()
        {
        }
    }
}
