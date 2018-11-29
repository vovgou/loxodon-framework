using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Tutorials
{
    [RequireComponent(typeof(Image))]
    public class AsyncSpriteLoader : MonoBehaviour
    {
        private Image target;
        private string spriteName;
        public Sprite defaultSprite;
        public Material defaultMaterial;
        public string spritePath;

        public string SpriteName
        {
            get { return this.spriteName; }
            set
            {
                if (this.spriteName == value)
                    return;

                this.spriteName = value;
                if (this.target != null)
                    this.OnSpriteChanged();
            }
        }

        protected virtual void OnEnable()
        {
            this.target = this.GetComponent<Image>();
        }

        protected virtual void OnSpriteChanged()
        {
            if (string.IsNullOrEmpty(this.spriteName))
            {
                this.target.sprite = null;
                this.target.material = null;
                return;
            }

            this.target.sprite = defaultSprite;
            this.target.material = defaultMaterial;

            StartCoroutine(LoadSprite());
        }

        /// <summary>
        /// Simulate the way asynchronous loading
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadSprite()
        {
            yield return new WaitForSeconds(1f); 

            Sprite[] sprites = Resources.LoadAll<Sprite>(this.spritePath);
            foreach(var sprite in sprites)
            {
                if(sprite.name.Equals(this.spriteName))
                {
                    this.target.sprite = sprite;
                    this.target.material = null;
                }
            }
        }
    }
}
