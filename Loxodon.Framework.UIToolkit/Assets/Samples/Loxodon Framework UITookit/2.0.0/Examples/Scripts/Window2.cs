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
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace Loxodon.Framework.Examples
{
    public class Window2 : UIToolkitWindow
    {
        protected override void OnCreate(IBundle bundle)
        {
            var bindingSet = this.CreateBindingSet(new Window2ViewMode());
            bindingSet.Bind(this.Q<Toggle>("toggle")).For(v => v.value).To(vm => vm.Toggle);
            bindingSet.Bind(this.Q<TextField>("username")).For(v => v.value, v => v.RegisterValueChangedCallback).To(vm => vm.Name);
            bindingSet.Build();
            this.Q<Button>("close").clicked += () => this.Dismiss();

        }

        public class Window2ViewMode : ViewModelBase
        {
            private string name = "Clark";
            private bool toggle = true;
            public string Name
            {
                get { return this.name; }
                set
                {
                    if (this.Set<string>(ref name, value))
                        Debug.LogFormat("Name:{0}", value);
                }
            }

            public bool Toggle
            {
                get { return this.toggle; }
                set { this.Set<bool>(ref toggle, value); }
            }


            public void OnClick()
            {
                Debug.LogFormat("Button OnClick");
            }
        }
    }
}
