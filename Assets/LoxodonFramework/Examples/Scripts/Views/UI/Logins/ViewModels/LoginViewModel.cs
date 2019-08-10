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

using System;
using System.Text.RegularExpressions;

using Loxodon.Log;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Prefs;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Commands;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Interactivity;

namespace Loxodon.Framework.Examples
{
    public class LoginViewModel : ViewModelBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ViewModelBase));

        private const string LAST_USERNAME_KEY = "LAST_USERNAME";

        private ObservableDictionary<string, string> errors = new ObservableDictionary<string, string>();
        private string username;
        private string password;
        private SimpleCommand loginCommand;
        private SimpleCommand cancelCommand;

        private Account account;

        private Preferences globalPreferences;
        private IAccountService accountService;
        private Localization localization;

        private InteractionRequest interactionFinished;
        private InteractionRequest<Notification> toastRequest;

        public LoginViewModel(IAccountService accountService, Localization localization, Preferences globalPreferences)
        {
            this.localization = localization;
            this.accountService = accountService;
            this.globalPreferences = globalPreferences;

            this.interactionFinished = new InteractionRequest(this);
            this.toastRequest = new InteractionRequest<Notification>(this);

            if (this.username == null)
            {
                this.username = globalPreferences.GetString(LAST_USERNAME_KEY, "");
            }

            this.loginCommand = new SimpleCommand(this.Login);
            this.cancelCommand = new SimpleCommand(() =>
            {
                this.interactionFinished.Raise();/* Request to close the login window */
            });
        }

        public IInteractionRequest InteractionFinished
        {
            get { return this.interactionFinished; }
        }

        public IInteractionRequest ToastRequest
        {
            get { return this.toastRequest; }
        }

        public ObservableDictionary<string, string> Errors { get { return this.errors; } }

        public string Username
        {
            get { return this.username; }
            set
            {
                if (this.Set<string>(ref this.username, value, "Username"))
                {
                    this.ValidateUsername();
                }
            }
        }

        public string Password
        {
            get { return this.password; }
            set
            {
                if (this.Set<string>(ref this.password, value, "Password"))
                {
                    this.ValidatePassword();
                }
            }
        }

        private bool ValidateUsername()
        {
            if (string.IsNullOrEmpty(this.username) || !Regex.IsMatch(this.username, "^[a-zA-Z0-9_-]{4,12}$"))
            {
                this.errors["username"] = localization.GetText("login.validation.username.error", "Please enter a valid username.");
                return false;
            }
            else {
                this.errors.Remove("username");
                return true;
            }
        }

        private bool ValidatePassword()
        {
            if (string.IsNullOrEmpty(this.password) || !Regex.IsMatch(this.password, "^[a-zA-Z0-9_-]{4,12}$"))
            {
                this.errors["password"] = localization.GetText("login.validation.password.error", "Please enter a valid password.");
                return false;
            }
            else {
                this.errors.Remove("password");
                return true;
            }
        }

        public ICommand LoginCommand
        {
            get { return this.loginCommand; }
        }

        public ICommand CancelCommand
        {
            get { return this.cancelCommand; }
        }

        public Account Account
        {
            get { return this.account; }
        }

        public void Login()
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("login start. username:{0} password:{1}", this.username, this.password);

            this.account = null;
            this.loginCommand.Enabled = false;/*by databinding, auto set button.interactable = false. */

            AsyncTask<Account> task;
            if (!(this.ValidateUsername() && this.ValidatePassword()))
                task = new AsyncTask<Account>(() => { return null; });
            else
            {
                task = new AsyncTask<Account>(() =>
                {
                    IAsyncResult<Account> result = this.accountService.Login(this.username, this.password);
                    Account account = result.Synchronized().WaitForResult();
                    if (account != null)
                    {
                        Context.GetApplicationContext().GetMainLoopExcutor().RunOnMainThread(() =>
                        {
                            globalPreferences.SetString(LAST_USERNAME_KEY, this.username);
                            globalPreferences.Save();
                        });
                    }
                    return account;
                });
            }

            task.OnPostExecute(account =>
            {
                if (account != null)
                {
                    /* login success */

                    this.account = account;
                    this.interactionFinished.Raise();/* Interaction completed, request to close the login window */
                }
                else {
                    /* Login failure */

                    var tipContent = this.localization.GetText("login.failure.tip", "Login failure.");
                    this.toastRequest.Raise(new Notification(tipContent));/* show toast */
                }
            }).OnError(e =>
            {
                if (log.IsErrorEnabled)
                    log.ErrorFormat("OnError:{0}", e);

                var tipContent = this.localization.GetText("login.exception.tip", "Login exception.");
                this.toastRequest.Raise(new Notification(tipContent));/* show toast */
            }).OnFinish(() =>
            {
                this.loginCommand.Enabled = true;/*by databinding, auto set button.interactable = true. */
            }).Start();
        }

        public IAsyncResult<Account> GetAccount()
        {
            return this.accountService.GetAccount(this.Username);
        }

    }
}