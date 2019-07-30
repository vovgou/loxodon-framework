using Loxodon.Framework.Localizations;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Localizations
{
    [RequireComponent(typeof(RawImage))]
    public class LocalizedRawImageInResources : AbstractLocalized<RawImage>
    {
        protected override void OnValueChanged(object sender, EventArgs e)
        {
            string path = (string)this.value.Value;
            this.StartCoroutine(DoLoad(path));
        }

        protected virtual IEnumerator DoLoad(string path)
        {
            var result = Resources.LoadAsync<Texture2D>(path);
            yield return result;
            this.target.texture = (Texture2D)result.asset;
        }
    }
}
