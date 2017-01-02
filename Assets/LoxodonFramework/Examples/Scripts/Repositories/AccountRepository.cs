using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Examples
{
	/// <summary>
	/// Simulate a account services, execute on the background thread.
	/// </summary>
	public class AccountRepository : IAccountRepository
	{
		private Dictionary<string,Account> cache = new Dictionary<string, Account> ();
		private IThreadExecutor executor;

		public AccountRepository ()
		{
			executor = new ThreadExecutor ();
			Account account = new Account (){ Username = "test", Password = "test", Created = DateTime.Now };
			cache.Add (account.Username, account);
		}

		public virtual IAsyncResult<Account> Get (string username)
		{
			return executor.Execute (() => {
				Account account = null;
				this.cache.TryGetValue (username, out account);
				return account;
			});
		}

		public virtual IAsyncResult<Account> Save (Account account)
		{
			return executor.Execute<Account> (new Action<IPromise<Account>> (promise => {
				if (cache.ContainsKey (account.Username)) {
					promise.SetException (new Exception ("The account already exists."));
					return;
				}

				cache.Add (account.Username, account);
				promise.SetResult (account);
				return;
			}));
		}

		public virtual IAsyncResult<Account> Update (Account account)
		{
			throw new NotImplementedException ();
		}

		public virtual IAsyncResult<bool> Delete (string username)
		{
			return executor.Execute (() => cache.Remove (username));
		}
	}
}