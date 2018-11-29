using Loxodon.Framework.Observables;
using System;
using UnityEngine;

namespace Loxodon.Framework.Localizations
{
    public abstract class AbstractLocalized<T> : MonoBehaviour where T : Component
    {
        [SerializeField]
        private string key;

        protected T target;
        protected IObservableProperty value;

        public string Key
        {
            get { return this.key; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Equals(this.key))
                    return;

                this.key = value;

                if (this.value != null)
                    this.value.ValueChanged -= OnValueChanged;

                if (this.target == null)
                    return;

                Localization localization = Localization.Current;
                this.value = localization.Get<IObservableProperty>(key);
                if (this.value != null)
                {
                    this.value.ValueChanged += OnValueChanged;
                    this.OnValueChanged(this.value, EventArgs.Empty);
                }
            }
        }

        protected virtual void OnEnable()
        {
            this.target = this.GetComponent<T>();
            if (this.target == null)
                return;

            Localization localization = Localization.Current;
            this.value = localization.Get<IObservableProperty>(key);
            if (this.value != null)
            {
                this.value.ValueChanged += OnValueChanged;
                this.OnValueChanged(this.value, EventArgs.Empty);
            }
        }

        protected virtual void OnDisable()
        {
            if (this.value != null)
                this.value.ValueChanged -= OnValueChanged;
        }

        protected abstract void OnValueChanged(object sender, EventArgs e);
    }
}
