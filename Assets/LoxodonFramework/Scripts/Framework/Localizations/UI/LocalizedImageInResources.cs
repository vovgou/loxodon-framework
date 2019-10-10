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

using Loxodon.Log;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Localizations
{
    [AddComponentMenu("Loxodon/Localization/LocalizedImageInResources")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class LocalizedImageInResources : AbstractLocalized<Image>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LocalizedImageInResources));

        protected override void OnValueChanged(object sender, EventArgs e)
        {
            object v = this.value.Value;
            if (v is Sprite)
            {
                this.target.sprite = (Sprite)v;
            }
            else if (v is string)
            {
                string path = (string)v;
                this.StartCoroutine(DoLoad(path));
            }
            else if (v != null)
            {
                if (log.IsErrorEnabled)
                    log.ErrorFormat("There is an invalid localization value \"{0}\" on the GameObject named \"{1}\".", v, this.name);
            }
        }

        protected virtual IEnumerator DoLoad(string path)
        {
            var result = Resources.LoadAsync<Sprite>(path);
            yield return result;
            this.target.sprite = (Sprite)result.asset;
        }
    }
}
