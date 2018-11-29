using System;
using UnityEngine;

namespace Loxodon.Framework.Localizations.UGUI
{
    [RequireComponent(typeof(TextMesh))]
    public class LocalizedTextMesh : AbstractLocalized<TextMesh>
    {
        protected override void OnValueChanged(object sender, EventArgs e)
        {
            this.target.text = (string)Convert.ChangeType(this.value.Value, typeof(string));
        }
    }
}
