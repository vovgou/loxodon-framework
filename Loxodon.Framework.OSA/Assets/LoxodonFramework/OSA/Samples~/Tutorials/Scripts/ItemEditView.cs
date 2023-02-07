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

using Loxodon.Framework.Binding;
using Loxodon.Framework.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Loxodon.Framework.Tutorials.OSA
{
    public class ItemEditView : UIView
    {
        public InputField titleInput;
        public Text title;
        public Slider colorSlider;
        public Button close;

        public ItemViewModel Item
        {
            get { return (ItemViewModel)this.GetDataContext(); }
            set { this.SetDataContext(value); }
        }

        protected override void Start()
        {
            var bindingSet = this.CreateBindingSet<ItemEditView, ItemViewModel>();
            bindingSet.Bind(titleInput).For(v => v.text,v=>v.onEndEdit).To(vm => vm.Title);
            bindingSet.Bind(title).For(v => v.color).To(vm => vm.Color);
            bindingSet.Bind(colorSlider).For(v => v.onValueChanged).To<float>(vm => vm.OnChangeColor);
            bindingSet.Build();

            this.close.onClick.AddListener(()=> {
                this.gameObject.SetActive(false);
            });
        }

    }
}
