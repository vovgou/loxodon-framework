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

using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;
using System;

namespace Loxodon.Framework.Examples
{
    public class AccountService : IAccountService
    {
        private IAccountRepository repository;

        public event EventHandler<LoginEventArgs> LoginFinished;

        public AccountService(IAccountRepository repository)
        {
            this.repository = repository;
        }


        public virtual IAsyncResult<Account> Register(Account account)
        {
            return this.repository.Save(account);
        }

        public virtual IAsyncResult<Account> Update(Account account)
        {
            return this.repository.Update(account);
        }

        public virtual IAsyncResult<Account> Login(string username, string password)
        {
            AsyncResult<Account> result = new AsyncResult<Account>();
            DoLogin(result, username, password);
            return result;
        }

        protected async void DoLogin(IPromise<Account> promise, string username, string password)
        {
            try
            {
                Account account = await this.GetAccount(username);
                if (account == null || !account.Password.Equals(password))
                {
                    promise.SetResult(null);
                    this.RaiseLoginFinished(false, null);
                }
                else
                {
                    promise.SetResult(account);
                    this.RaiseLoginFinished(true, account);
                }
            }
            catch (Exception e)
            {
                promise.SetException(e);
                this.RaiseLoginFinished(false, null);
            }
        }

        public virtual IAsyncResult<Account> GetAccount(string username)
        {
            return this.repository.Get(username);
        }

        protected virtual void RaiseLoginFinished(bool succeed, Account account)
        {
            try
            {
                if (this.LoginFinished != null)
                    this.LoginFinished(this, new LoginEventArgs(succeed, account));
            }
            catch (Exception) { }
        }
    }
}