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

using UnityEngine;

namespace Loxodon.Framework.Views
{
    public enum ScriptReferenceType
    {
        TextAsset,
        Filename
    }

    [System.Serializable]
    public class ScriptReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        [SerializeField]
        private Object cachedAsset;
#endif
        [SerializeField]
        protected TextAsset text;

        [SerializeField]
        protected string filename;

        [SerializeField]
        protected ScriptReferenceType type = ScriptReferenceType.TextAsset;

        public virtual ScriptReferenceType Type
        {
            get { return this.type; }
        }

        public virtual TextAsset Text
        {
            get { return this.text; }
        }

        public virtual string Filename
        {
            get { return this.filename; }
        }

        public void OnAfterDeserialize()
        {
            Clear();
        }

        public void OnBeforeSerialize()
        {
            Clear();
        }

        protected virtual void Clear()
        {
#if !UNITY_EDITOR
            switch (type)
            {
                case ScriptReferenceType.TextAsset:
                    this.filename = null;
                    break;
                case ScriptReferenceType.Filename:
                    this.text = null;
                    break;
            }
#endif
        }
    }
}