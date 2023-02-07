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
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using Loxodon.Log;
using UnityEngine.UI;

namespace Loxodon.Framework.Examples
{
    public class LoginWindow : Window
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(LoginWindow));

        public InputField username;
        public InputField password;
        public Text usernameErrorPrompt;
        public Text passwordErrorPrompt;
        public Button confirmButton;
        public Button cancelButton;


        protected override void OnCreate(IBundle bundle)
        {
            BindingSet<LoginWindow, LoginViewModel> bindingSet = this.CreateBindingSet<LoginWindow, LoginViewModel>();
            bindingSet.Bind().For(v => v.OnInteractionFinished).To(vm => vm.InteractionFinished);
            bindingSet.Bind().For(v => v.OnToastShow).To(vm => vm.ToastRequest);

            bindingSet.Bind(this.username).For(v => v.text, v => v.onEndEdit).To(vm => vm.Username).TwoWay();
            bindingSet.Bind(this.usernameErrorPrompt).For(v => v.text).To(vm => vm.Errors["username"]).OneWay();
            bindingSet.Bind(this.password).For(v => v.text, v => v.onEndEdit).To(vm => vm.Password).TwoWay();
            bindingSet.Bind(this.passwordErrorPrompt).For(v => v.text).To(vm => vm.Errors["password"]).OneWay();
            bindingSet.Bind(this.confirmButton).For(v => v.onClick).To(vm => vm.LoginCommand);
            bindingSet.Bind(this.cancelButton).For(v => v.onClick).To(vm => vm.CancelCommand);
            bindingSet.Build();
        }

        public virtual void OnInteractionFinished(object sender, InteractionEventArgs args)
        {
            this.Dismiss();
        }

        public virtual void OnToastShow(object sender, InteractionEventArgs args)
        {
            ToastNotification notification = args.Context as ToastNotification;
            if (notification == null)
                return;

            Toast.Show(this, notification.Message, notification.Duration);
        }
    }
}