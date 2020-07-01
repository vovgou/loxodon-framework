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