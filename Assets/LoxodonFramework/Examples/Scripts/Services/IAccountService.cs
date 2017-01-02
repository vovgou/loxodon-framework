using UnityEngine;
using System.Collections;
using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Examples
{
	public interface IAccountService
	{
		IAsyncResult<Account> Register (Account account);

		IAsyncResult<Account> Login (string username, string password);

		IAsyncResult<Account> GetAccount (string username);
	}
}