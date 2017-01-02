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