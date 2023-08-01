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
using System.Collections;
using System.Threading;
using UnityEngine;

using Loxodon.Log;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Commands;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Contexts;
#if NETFX_CORE
using System.Threading.Tasks;
#endif

namespace Loxodon.Framework.Examples
{
    public class StartupViewModel : ViewModelBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StartupViewModel));

        private ProgressBar progressBar = new ProgressBar();
        private SimpleCommand command;
        private Localization localization;

        //public InteractionRequest<LoginViewModel> LoginRequest;
        public AsyncInteractionRequest<WindowNotification> LoginRequest { get; private set; }
        public InteractionRequest DismissRequest { get; private set; }


        public StartupViewModel() : this(null)
        {
        }

        public StartupViewModel(IMessenger messenger) : base(messenger)
        {
            ApplicationContext context = Context.GetApplicationContext();
            this.localization = context.GetService<Localization>();
            var accountService = context.GetService<IAccountService>();
            var globalPreferences = context.GetGlobalPreferences();

            //this.LoginRequest = new InteractionRequest<LoginViewModel>(this);          
            this.LoginRequest = new AsyncInteractionRequest<WindowNotification>(this);
            this.DismissRequest = new InteractionRequest(this);

            var loginViewModel = new LoginViewModel(accountService, localization, globalPreferences);
            //this.command = new SimpleCommand(() =>
            //{
            //    this.LoginRequest.Raise(loginViewModel, vm =>
            //    {
            //        this.command.Enabled = true;

            //        if (vm.Account != null)
            //            this.LoadScene();
            //    });
            //});

            this.command = new SimpleCommand(async () =>
            {
                this.command.Enabled = false;
                await this.LoginRequest.Raise(WindowNotification.CreateShowNotification(loginViewModel, false, true));
                this.command.Enabled = true;
                if (loginViewModel.Account != null)
                    this.LoadScene();
            });
        }

        public ProgressBar ProgressBar
        {
            get { return this.progressBar; }
        }

        public ICommand Click
        {
            get { return this.command; }
        }

        public void OnClick()
        {
            log.Debug("onClick");
        }

        /// <summary>
        /// Simulate a unzip task.
        /// </summary>
        public async void Unzip()
        {
            this.command.Enabled = false;
            this.progressBar.Enable = true;
            this.ProgressBar.Tip = R.startup_progressbar_tip_unziping;

            try
            {
                var progress = 0f;
                while (progress < 1f)
                {
                    progress += 0.01f;
                    this.ProgressBar.Progress = progress;/* update progress */
                    await new WaitForSecondsRealtime(0.02f);
                }
            }
            finally
            {
                this.command.Enabled = true;
                this.progressBar.Enable = false;
                this.progressBar.Tip = "";
                this.command.Execute(null);
            }
        }

        /// <summary>
        /// Simulate a loading task.
        /// </summary>
        public async void LoadScene()
        {
            try
            {
                this.progressBar.Enable = true;
                this.ProgressBar.Tip = R.startup_progressbar_tip_loading;

                ResourceRequest request = Resources.LoadAsync<GameObject>("Prefabs/Cube");
                while (!request.isDone)
                {
                    this.ProgressBar.Progress = request.progress;/* update progress */
                    await new WaitForSecondsRealtime(0.02f);
                }

                GameObject sceneTemplate = (GameObject)request.asset;
                GameObject.Instantiate(sceneTemplate);
            }
            finally
            {
                this.ProgressBar.Tip = "";
                this.progressBar.Enable = false;
                this.DismissRequest.Raise();
            }
        }
    }
}