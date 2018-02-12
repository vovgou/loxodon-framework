using UnityEngine;
using UnityEngine.UI;
using System;

using Loxodon.Log;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Commands;

namespace Loxodon.Framework.Examples
{
    public class StartupWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Text progressBarText;
        public Slider progressBarSlider;
        public Text tipText;
        public Button button;

        private StartupViewModel viewModel;
        private IDisposable subscription;

        private SimpleCommand command;
        private IUIViewLocator viewLocator;

        protected override void OnCreate(IBundle bundle)
        {
            this.viewLocator = Context.GetApplicationContext().GetService<IUIViewLocator>();
            this.command = new SimpleCommand(OpenLoginWindow);
            this.viewModel = new StartupViewModel();
            this.viewModel.Click = this.command;
            //this.subscription = this.viewModel.Messenger.Subscribe ();

            //		this.SetDataContext (viewModel);

            /* databinding, Bound to the ViewModel. */
            BindingSet<StartupWindow, StartupViewModel> bindingSet = this.CreateBindingSet(viewModel);
            bindingSet.Bind(this.progressBarSlider).For("value", "onValueChanged").To("ProgressBar.Progress").TwoWay();
            //			bindingSet.Bind (this.progressBarSlider).For (v => v.value, v => v.onValueChanged).To (vm => vm.ProgressBar.Progress).TwoWay ();

            /* //by the way,You can expand your attributes. 		 
		ProxyFactory proxyFactory = ProxyFactory.Default;
		PropertyInfo info = typeof(GameObject).GetProperty ("activeSelf");
		proxyFactory.Register (new ProxyPropertyInfo<GameObject, bool> (info, go => go.activeSelf, (go, value) => go.SetActive (value)));
		*/

            bindingSet.Bind(this.progressBarSlider.gameObject).For(v => v.activeSelf).To(vm => vm.ProgressBar.Enable).OneWay();
            bindingSet.Bind(this.progressBarText).For(v => v.text).ToExpression(vm => string.Format("{0}%", Mathf.FloorToInt(vm.ProgressBar.Progress * 100f))).OneWay();/* expression binding,support only OneWay mode. */
            bindingSet.Bind(this.tipText).For(v => v.text).To(vm => vm.ProgressBar.Tip).OneWay();

            //bindingSet.Bind (this.button).For (v => v.onClick).To (vm=>vm.OnClick()).OneWay (); //Method binding,only bound to the onClick event.
            bindingSet.Bind(this.button).For(v => v.onClick).To(vm => vm.Click).OneWay();//Command binding,bound to the onClick event and interactable property.
            bindingSet.Build();

            this.Unzip();
        }

        public override void DoDismiss()
        {
            base.DoDismiss();
            if (this.subscription != null)
            {
                this.subscription.Dispose();
                this.subscription = null;
            }
        }

        /// <summary>
        ///  Simulate a loading task.
        /// </summary>
        public void Unzip()
        {
            Loading loading = null;
            IProgressTask<float> task = this.viewModel.Unzip();
            task.OnPreExecute(() =>
            {
                loading = Loading.Show();
                this.command.Enabled = false;/*by databinding, auto set button.interactable = false. */
            }).OnFinish(() =>
            {
                loading.Dispose();
                this.OpenLoginWindow();
                this.command.Enabled = true;/*by databinding, auto set button.interactable = true. */
            }).Start(30);
        }


        protected void OpenLoginWindow()
        {
            try
            {
                this.command.Enabled = false;
                LoginWindow loginWindow = viewLocator.LoadWindow<LoginWindow>(this.WindowManager, "UI/Logins/Login");
                loginWindow.OnLoginFinished += OnLoginFinished;
                loginWindow.Create();
                loginWindow.Show();
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.Warn(e.StackTrace);
            }
        }

        protected void OnLoginFinished(bool result, Account account)
        {
            this.command.Enabled = true;
            if (result)
            {
                /* login successed */
                if (log.IsDebugEnabled)
                    log.Debug("login successed.");

                IProgressTask<float> task = this.viewModel.LoadScene();
                task.OnPreExecute(() =>
                {
                    this.command.Enabled = false;/*by databinding, auto set button.interactable = false. */
                }).OnFinish(() =>
                {
                    this.command.Enabled = true;/*by databinding, auto set button.interactable = true. */
                    this.Dismiss();
                }).Start();

            }
            else {
                /* login cancelled */
                if (log.IsDebugEnabled)
                    log.Debug("login cancelled.");
            }
        }
    }
}