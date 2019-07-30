using Loxodon.Framework.Localizations;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Localizations
{
    [RequireComponent(typeof(Image))]
    public class LocalizedImageInResources : AbstractLocalized<Image>
    {
        protected override void OnValueChanged(object sender, EventArgs e)
        {
            string path = (string)this.value.Value;
            this.StartCoroutine(DoLoad(path));
        }

        protected virtual IEnumerator DoLoad(string path)
        {
            var result = Resources.LoadAsync<Sprite>(path);
            yield return result;
            this.target.sprite = (Sprite)result.asset;
        }
    }
}
