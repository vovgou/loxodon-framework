using System;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Localizations
{
    [RequireComponent(typeof(Text))]
    public class LocalizedText : AbstractLocalized<Text>
    {
        protected override void OnValueChanged(object sender, EventArgs e)
        {
            this.target.text = (string)Convert.ChangeType(this.value.Value, typeof(string));
        }
    }
}
