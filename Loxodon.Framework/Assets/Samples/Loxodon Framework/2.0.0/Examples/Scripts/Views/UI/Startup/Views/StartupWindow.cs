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
using UnityEngine.UI;
using System;

using Loxodon.Log;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views.InteractionActions;

namespace Loxodon.Framework.Examples
{
    public class StartupWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StartupWindow));

        public Text progressBarText;
        public Slider progressBarSlider;
        public Text tipText;
        public Button button;

        private StartupViewModel viewModel;
        private IUIViewLocator viewLocator;
        private AsyncWindowInteractionAction loginWindowInteractionAction;

        protected override void OnCreate(IBundle bundle)
        {
            this.viewLocator = Context.GetApplicationContext().GetService<IUIViewLocator>();
            this.loginWindowInteractionAction = new AsyncWindowInteractionAction("UI/Logins/Login", viewLocator,this.WindowManager);
            this.viewModel = new StartupViewModel();

            /* databinding, Bound to the ViewModel. */
            BindingSet<StartupWindow, StartupViewModel> bindingSet = this.CreateBindingSet(viewModel);
            //bindingSet.Bind().For(v => v.OnOpenLoginWindow).To(vm => vm.LoginRequest);
            bindingSet.Bind().For(v => v.loginWindowInteractionAction).To(vm => vm.LoginRequest);
            bindingSet.Bind().For(v => v.OnDismissRequest).To(vm => vm.DismissRequest);

            bindingSet.Bind(this.progressBarSlider).For("value", "onValueChanged").To("ProgressBar.Progress").TwoWay();
            //bindingSet.Bind (this.progressBarSlider).For (v => v.value, v => v.onValueChanged).To (vm => vm.ProgressBar.Progress).TwoWay ();

            /* //by the way,You can expand your attributes. 		 
		        ProxyFactory proxyFactory = ProxyFactory.Default;
		        PropertyInfo info = typeof(GameObject).GetProperty ("activeSelf");
		        proxyFactory.Register (new ProxyPropertyInfo<GameObject, bool> (info, go => go.activeSelf, (go, value) => go.SetActive (value)));
		    */

            bindingSet.Bind(this.progressBarSlider.gameObject).For(v => v.activeSelf).To(vm => vm.ProgressBar.Enable).OneWay();
            bindingSet.Bind(this.progressBarText).For(v => v.text).ToExpression(vm => string.Format("{0}%", Mathf.FloorToInt(vm.ProgressBar.Progress * 100f))).OneWay();/* expression binding,support only OneWay mode. */
            bindingSet.Bind(this.tipText).For(v => v.text).To(vm => vm.ProgressBar.Tip).OneWay();

            //bindingSet.Bind(this.button).For(v => v.onClick).To(vm=>vm.OnClick()).OneWay(); //Method binding,only bound to the onClick event.
            bindingSet.Bind(this.button).For(v => v.onClick).To(vm => vm.Click).OneWay();//Command binding,bound to the onClick event and interactable property.
            bindingSet.Build();

            this.viewModel.Unzip();
        }

        protected void OnDismissRequest(object sender, InteractionEventArgs args)
        {
            this.Dismiss();
        }

        //// Use AsyncWindowInteractionAction instead of this method.
        //protected void OnOpenLoginWindow(object sender, InteractionEventArgs args)
        //{
        //    try
        //    {
        //        LoginWindow loginWindow = viewLocator.LoadWindow<LoginWindow>(this.WindowManager, "UI/Logins/Login");
        //        var callback = args.Callback;
        //        var loginViewModel = args.Context;

        //        if (callback != null)
        //        {
        //            EventHandler handler = null;
        //            handler = (window, e) =>
        //            {
        //                loginWindow.OnDismissed -= handler;
        //                if (callback != null)
        //                    callback();
        //            };
        //            loginWindow.OnDismissed += handler;
        //        }

        //        loginWindow.SetDataContext(loginViewModel);
        //        loginWindow.Create();
        //        loginWindow.Show();
        //    }
        //    catch (Exception e)
        //    {
        //        if (log.IsWarnEnabled)
        //            log.Warn(e);
        //    }
        //}
    }
}