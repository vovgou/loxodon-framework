using System;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Observables;

namespace Loxodon.Framework.Localizations
{
    public class LocalizedText : MonoBehaviour
    {
        private Text text;
        private IObservableProperty value;
        public string key;
        void Awake()
        {
            Localization localization = Localization.Current;
            this.value = localization.Get<IObservableProperty>(key);
            this.text = this.GetComponent<Text>();
            if (this.value != null)
            {
                this.value.ValueChanged += OnValueChanged;
                this.text.text = (string)Convert.ChangeType(this.value.Value, typeof(string));
            }
        }

        void OnDestroy()
        {
            if (this.value != null)
                this.value.ValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            this.text.text = (string)Convert.ChangeType(this.value.Value, typeof(string));
        }
    }
}
