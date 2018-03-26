using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Loxodon.Log;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Commands;

namespace Loxodon.Framework.Examples
{
	public class LoginWindow  : Window
	{
		private static readonly ILog log = LogManager.GetLogger (typeof(LoginWindow));

		public InputField username;
		public InputField password;
		public Text usernameErrorPrompt;
		public Text passwordErrorPrompt;
		public Button confirmButton;
		public Button cancelButton;

		private LoginViewModel viewModel;
		private SimpleCommand loginCommand;
		private Localization localization;

		public event Action<bool,Account> OnLoginFinished;

		protected override void OnCreate (IBundle bundle)
		{
			ApplicationContext context = Context.GetApplicationContext ();
			this.localization = context.GetService<Localization> ();
			var accountService = context.GetService<IAccountService> ();
			var globalPreferences = context.GetGlobalPreferences ();
			this.viewModel = new LoginViewModel (accountService, localization, globalPreferences);
			this.loginCommand = new SimpleCommand (Login);
			this.viewModel.LoginCommand = this.loginCommand;

			BindingSet<LoginWindow,LoginViewModel> bindingSet = this.CreateBindingSet (viewModel);
			bindingSet.Bind (this.username).For (v => v.text, v => v.onEndEdit).To (vm => vm.Username).TwoWay ();
			bindingSet.Bind (this.usernameErrorPrompt).For (v => v.text).To (vm => vm.Errors ["username"]).OneWay ();
			bindingSet.Bind (this.password).For (v => v.text, v => v.onEndEdit).To (vm => vm.Password).TwoWay ();
			bindingSet.Bind (this.passwordErrorPrompt).For (v => v.text).To (vm => vm.Errors ["password"]).OneWay ();
			bindingSet.Bind (this.confirmButton).For (v => v.onClick).To (vm => vm.LoginCommand);
			bindingSet.Build ();

			this.cancelButton.onClick.AddListener (() => {			
				this.Dismiss ();
				this.RaiseOnLoginFinish (false, null);
			});
		}

		protected virtual void Login ()
		{
			Loading loading = null;
			IAsyncTask<Account> task = this.viewModel.Login ();
			task.OnPreExecute (() => {
				loading = Loading.Show ();
				this.loginCommand.Enabled = false;/*by databinding, auto set button.interactable = false. */
			}).OnPostExecute ((account) => {
				if (account != null) {
					/* login success */
					this.Dismiss ();
					this.RaiseOnLoginFinish (true, account);
				} else {
					/* Login failure */
					var tipContent = this.localization.GetText ("login.failure.tip", "Login failure.");
					Toast.Show (this, tipContent, 2f);
				}
			}).OnError (e => {
			
				if (log.IsErrorEnabled)
					log.Error ("OnError:" + e.StackTrace);
			
				var tipContent = this.localization.GetText ("login.exception.tip", "Login exception.");
				Toast.Show (this, tipContent, 2f);
			}).OnFinish (() => {
				loading.Dispose ();
				this.loginCommand.Enabled = true;/*by databinding, auto set button.interactable = true. */
			}).Start ();
		}

		protected void RaiseOnLoginFinish (bool result, Account account)
		{
			if (this.OnLoginFinished != null) {
				this.OnLoginFinished (result, account);
			}
		}
	}
}