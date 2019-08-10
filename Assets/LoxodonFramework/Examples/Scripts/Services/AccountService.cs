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
using System;
using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Examples
{
	public class AccountService : IAccountService
	{
		private IAccountRepository repository;
		private IThreadExecutor executor;

		public AccountService (IAccountRepository repository)
		{
			this.repository = repository;
			this.executor = new ThreadExecutor ();
		}


		public virtual IAsyncResult<Account> Register (Account account)
		{
			return this.repository.Save (account);
		}

		public virtual IAsyncResult<Account> Login (string username, string password)
		{
			return this.executor.Execute (new Action<IPromise<Account>> ((promise) => {
				try {
					IAsyncResult<Account> accountResult = this.GetAccount (username);
					Account account = accountResult.Synchronized().WaitForResult ();
					if (account == null || !account.Password.Equals (password)) {
						promise.SetResult (null);
					} else {
						promise.SetResult (account);
					}
				} catch (Exception e) {
					promise.SetException (e);
				}
			}));
		}

		public virtual IAsyncResult<Account> GetAccount (string username)
		{
			return this.repository.Get (username);
		}
	}
}