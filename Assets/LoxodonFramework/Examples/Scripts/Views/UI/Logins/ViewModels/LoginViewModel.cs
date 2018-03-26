using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;

using Loxodon.Log;
using Loxodon.Framework.Execution;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Prefs;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Commands;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Observables;

namespace Loxodon.Framework.Examples
{
	public class LoginViewModel : ViewModelBase
	{
		private static readonly ILog log = LogManager.GetLogger (typeof(ViewModelBase));

		private const string LAST_USERNAME_KEY = "LAST_USERNAME";

		private ObservableDictionary<string, string> errors = new ObservableDictionary<string, string> ();
		private string username;
		private string password;
		private ICommand command;

		private Preferences globalPreferences;
		private IAccountService accountService;
		private Localization localization;

		public LoginViewModel (IAccountService accountService, Localization localization, Preferences globalPreferences)
		{
			this.localization = localization;
			this.accountService = accountService;
			this.globalPreferences = globalPreferences;

			if (this.username == null) {
				this.username = globalPreferences.GetString (LAST_USERNAME_KEY, "");
			}
		}

		public ObservableDictionary<string, string> Errors{ get { return this.errors; } }

		public string Username {
			get{ return this.username; }
			set { 
				if (this.Set<string> (ref this.username, value, "Username")) {
					this.ValidateUsername ();
				}
			}
		}

		public string Password {
			get{ return this.password; }
			set {
				if (this.Set<string> (ref this.password, value, "Password")) {
					this.ValidatePassword ();
				}
			}
		}

		private bool ValidateUsername ()
		{
			if (string.IsNullOrEmpty (this.username) || !Regex.IsMatch (this.username, "^[a-zA-Z0-9_-]{4,12}$")) {
				this.errors ["username"] = localization.GetText ("login.validation.username.error", "Please enter a valid username.");
				return false;
			} else {
				this.errors.Remove ("username");
				return true;
			}
		}

		private bool ValidatePassword ()
		{
			if (string.IsNullOrEmpty (this.password) || !Regex.IsMatch (this.password, "^[a-zA-Z0-9_-]{4,12}$")) {
				this.errors ["password"] = localization.GetText ("login.validation.password.error", "Please enter a valid password.");
				return false;
			} else {
				this.errors.Remove ("password");
				return true;
			}
		}

		public ICommand LoginCommand {
			get{ return this.command; }
			set{ this.command = value; }
		}

		public IAsyncTask<Account> Login ()
		{
			if (log.IsDebugEnabled)
				log.DebugFormat ("login start. username:{0} password:{1}", this.username, this.password);

			if (!(this.ValidatePassword () && this.ValidateUsername ()))
				return new AsyncTask<Account> (()=>{ return null; });
		
			AsyncTask<Account> task = new AsyncTask<Account> (() => {
				IAsyncResult<Account> result = this.accountService.Login (this.username, this.password);
				Account account = result.Synchronized().WaitForResult ();
				if (account != null) {
					Context.GetApplicationContext().GetMainLoopExcutor().RunOnMainThread(()=>{
						globalPreferences.SetString (LAST_USERNAME_KEY, this.username);
						globalPreferences.Save ();
					});
				}
				return account;
			});
			return task;
		}

		public IAsyncResult<Account> GetAccount ()
		{
			return this.accountService.GetAccount (this.Username);
		}

	}
}