using UnityEngine;
using System.Collections;

using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Examples
{
	public interface IAccountRepository
	{
		IAsyncResult<Account> Get (string username);

		IAsyncResult<Account> Save (Account account);

		IAsyncResult<Account> Update (Account account);

		IAsyncResult<bool> Delete (string username);
	}
}