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

using Loxodon.Framework.Observables;
using Loxodon.Log;
using System;
using UnityEngine;

namespace Loxodon.Framework.Localizations
{
    [DefaultExecutionOrder(100)]
    public abstract class AbstractLocalized<T> : MonoBehaviour where T : Component
    {
        //private static readonly ILog log = LogManager.GetLogger("AbstractLocalized");

        [SerializeField]
        private string key;

        protected T target;
        protected IObservableProperty value;

        protected virtual void OnKeyChanged()
        {
            if (this.value != null)
                this.value.ValueChanged -= OnValueChanged;

            if (!this.enabled || this.target == null || string.IsNullOrEmpty(key))
                return;

            Localization localization = Localization.Current;
            this.value = localization.GetValue(key);

            //if (this.value == null)
            //{
            //    if (Application.isPlaying && log.IsErrorEnabled)
            //        log.ErrorFormat("There is an invalid localization key \"{0}\" on the {1} object named \"{2}\".", key, typeof(T).Name, this.name);
            //    return;
            //}

            this.value.ValueChanged += OnValueChanged;
            this.OnValueChanged(this.value, EventArgs.Empty);
        }

        public string Key
        {
            get { return this.key; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Equals(this.key))
                    return;

                this.key = value;
                this.OnKeyChanged();
            }
        }

        protected virtual void OnEnable()
        {
            if (this.target == null)
                this.target = this.GetComponent<T>();

            if (this.target == null)
                return;

            this.OnKeyChanged();
        }

        protected virtual void OnDisable()
        {
            if (this.value != null)
            {
                this.value.ValueChanged -= OnValueChanged;
                this.value = null;
            }
        }

        protected abstract void OnValueChanged(object sender, EventArgs e);
    }
}
