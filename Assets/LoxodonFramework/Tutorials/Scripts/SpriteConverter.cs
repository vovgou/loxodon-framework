using Loxodon.Framework.Binding.Converters;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loxodon.Framework.Tutorials
{
    public class SpriteConverter : IConverter
    {
        private Dictionary<string, Sprite> sprites;

        public SpriteConverter(Dictionary<string, Sprite> sprites)
        {
            this.sprites = sprites;
        }

        public object Convert(object value)
        {
            Sprite sprite;
            sprites.TryGetValue((string)value, out sprite);
            return sprite;
        }

        public object ConvertBack(object value)
        {
            throw new NotImplementedException();
        }
    }
}
