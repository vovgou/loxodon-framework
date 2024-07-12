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

using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Tutorials
{
    public class AsyncImage : Image
    {
        private string spriteName;
        private Material originMaterial;
        private CancellationTokenSource source;
        public Sprite loadingSprite;
        public Material loadingMaterial;

        public string SpriteName
        {
            get { return this.spriteName; }
            set
            {
                if (string.Equals(this.spriteName, value))
                    return;

                this.spriteName = value;
                this.OnSpriteNameChanged(this.spriteName);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.originMaterial = this.material;
        }

        protected async void OnSpriteNameChanged(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                this.material = originMaterial;
                this.sprite = null;
                return;
            }

            if (this.source != null)
                this.source.Cancel();

            this.source = new CancellationTokenSource();
            CancellationToken token = this.source.Token;
            try
            {
                this.sprite = loadingSprite;
                this.material = loadingMaterial;
                Sprite sprite = await LoadSprite(spriteName);
                if (!token.IsCancellationRequested)
                {
                    this.material = originMaterial;
                    this.sprite = sprite;
                    this.source = null;
                }
            }
            catch
            {
                if (!token.IsCancellationRequested)
                {
                    this.material = originMaterial;
                    this.sprite = null;
                    this.source = null;
                }
            }
        }

        protected async Task<Sprite> LoadSprite(string spriteName)
        {
            return (Sprite)await Resources.LoadAsync<Sprite>(spriteName);
        }
    }
}
