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
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace Loxodon.Framework.Examples
{
    public class UGUIWindow1 : UIToolkitWindow
    {
        protected override void OnCreate(IBundle bundle)
        {
            var bindingSet = this.CreateBindingSet(new UGUIWindowViewMode());
            bindingSet.Bind(this.Q<Button>()).For(v=>v.clickable).To(vm => vm.Click);
            //bindingSet.Bind<Button>().For(v => v.clickable).To(vm => vm.Click);
            bindingSet.Bind().For(v => v.OnOpenDialogWindow).To(vm => vm.OpenRequest);
            bindingSet.Build();
        }

        protected void OnOpenDialogWindow(object sender, InteractionEventArgs args)
        {
            var callback = args.Callback;
            AlertDialog.ShowMessage("测试", "标题", "OK", r =>
            {
                if (callback != null)
                    callback();
            });
        }
    }

    public class UGUIWindowViewMode : ViewModelBase
    {
        private string name;
        private SimpleCommand command;
        private InteractionRequest openRequest;
        public UGUIWindowViewMode()
        {
            this.openRequest = new InteractionRequest(this);
            this.command = new SimpleCommand(() =>
            {
                this.command.Enabled = false;
                this.openRequest.Raise(()=> {
                    this.command.Enabled = true;
                });               
            });
        }

        public string Name
        {
            get { return this.name; }
            set { this.Set<string>(ref name, value); }
        }

        public IInteractionRequest OpenRequest
        {
            get { return this.openRequest; }
        }

        public ICommand Click
        {
            get { return this.command; }
        }

        public void OnClick()
        {
            Debug.LogFormat("Button OnClick");
        }
    }
}
