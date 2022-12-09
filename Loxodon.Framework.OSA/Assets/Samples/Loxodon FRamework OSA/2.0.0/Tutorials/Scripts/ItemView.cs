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
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Views;
using UnityEngine.UI;

namespace Loxodon.Framework.Tutorials.OSA
{
    public class ItemView : UIView
    {
        public Image background;
        public Text titleText;
        public Image border;
        public Button selectButton;
        public Button clickButton;
        protected override void Start()
        {
            BindingSet<ItemView, ItemViewModel> bindingSet = this.CreateBindingSet<ItemView, ItemViewModel>();
            bindingSet.Bind(this.titleText).For(v => v.text).To(vm => vm.Title).OneWay();
            bindingSet.Bind(this.background).For(v => v.color).To(vm => vm.Color).OneWay();
            bindingSet.Bind(this.border).For(v => v.enabled).To(vm => vm.Selected).OneWay();
            bindingSet.Bind(this.selectButton).For(v => v.onClick).To(vm => vm.SelectCommand).CommandParameter(this.GetDataContext);
            if (this.clickButton != null)
                bindingSet.Bind(this.clickButton).For(v => v.onClick).To(vm => vm.ClickCommand).CommandParameter(this.GetDataContext);
            bindingSet.Build();
        }
    }
}