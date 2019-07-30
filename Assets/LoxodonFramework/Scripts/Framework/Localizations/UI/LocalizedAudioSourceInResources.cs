using Loxodon.Framework.Localizations;
using System;
using System.Collections;
using UnityEngine;

namespace Loxodon.Framework.Localizations
{
    [RequireComponent(typeof(AudioSource))]
    public class LocalizedAudioSourceInResources : AbstractLocalized<AudioSource>
    {
        protected override void OnValueChanged(object sender, EventArgs e)
        {
            string path = (string)this.value.Value;
            this.StartCoroutine(DoLoad(path));
        }

        protected virtual IEnumerator DoLoad(string path)
        {
            var result = Resources.LoadAsync<AudioClip>(path);
            yield return result;
            this.target.clip = (AudioClip)result.asset;
        }
    }
}
