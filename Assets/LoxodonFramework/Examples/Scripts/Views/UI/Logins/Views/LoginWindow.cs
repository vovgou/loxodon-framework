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
            bindingSet.Bind().For(v => v.OnInteractionFinished(null, null)).To(vm => vm.InteractionFinished);
            bindingSet.Bind().For(v => v.OnToastShow(null, null)).To(vm => vm.ToastRequest);

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
            Notification notification = args.Context as Notification;
            if (notification == null)
                return;

            Toast.Show(this, notification.Message, 2f);
        }
    }
}