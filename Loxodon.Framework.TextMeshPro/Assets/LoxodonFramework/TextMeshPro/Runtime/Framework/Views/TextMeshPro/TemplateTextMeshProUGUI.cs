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

using Loxodon.Framework.TextFormatting;
using TMPro;
using UnityEngine;
using static Loxodon.Framework.Views.TextMeshPro.IFormattableText;

namespace Loxodon.Framework.Views.TextMeshPro
{
    public class TemplateTextMeshProUGUI : TextMeshProUGUI
    {
        [SerializeField]
        [TextArea(5, 10)]
        private string m_Template;
        private object data;
        private TextTemplateBinding templateBinding;

        protected TextTemplateBinding Binding
        {
            get
            {
                if (templateBinding == null)
                    templateBinding = new TextTemplateBinding(SetText);
                return templateBinding;
            }
        }

        public string Template
        {
            get { return this.m_Template; }
            set
            {
                if (string.Equals(this.m_Template, value))
                    return;

                this.m_Template = value;
                Binding.Template = this.m_Template;
            }
        }
        public object Data
        {
            get { return this.data; }
            set
            {
                if (Equals(this.data, value))
                    return;

                this.data = value;
                Binding.Data = this.data;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Initialize();
        }

        public override void SetAllDirty()
        {
            base.SetAllDirty();
            Initialize();
        }

        protected virtual void Initialize()
        {
            if (this.data == null)
                SetText(BUFFER.Clear().Append(m_Template));
        }

        protected override void OnDestroy()
        {
            if (templateBinding != null)
            {
                templateBinding.Dispose();
                templateBinding = null;
            }
            base.OnDestroy();
        }
    }
}
