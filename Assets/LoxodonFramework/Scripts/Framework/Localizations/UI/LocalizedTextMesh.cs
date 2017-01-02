using System;
using UnityEngine;
using Loxodon.Framework.Observables;

namespace Loxodon.Framework.Localizations.UGUI
{
    public class LocalizedTextMesh : MonoBehaviour
    {
        private TextMesh text;
        private IObservableProperty value;
        public string key;
        void Awake()
        {
            Localization localization = Localization.Current;
            this.value = localization.Get<IObservableProperty>(key);
            this.text = this.GetComponent<TextMesh>();
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
