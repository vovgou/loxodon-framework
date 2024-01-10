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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Loxodon.Framework.Examples
{
    /// <summary>
    /// Simulate a account services, execute on the background thread.
    /// </summary>
    public class AccountRepository : IAccountRepository
    {
        private Dictionary<string, Account> cache = new Dictionary<string, Account>();

        public AccountRepository()
        {
            Account account = new Account() { Username = "test", Password = "test", Created = DateTime.Now };
            cache.Add(account.Username, account);
        }

        public virtual Task<Account> Get(string username)
        {
            Account account = null;
            this.cache.TryGetValue(username, out account);
            return Task.FromResult(account);
        }

        public virtual async Task<Account> Save(Account account)
        {
            if (cache.ContainsKey(account.Username))
                throw new Exception("The account already exists.");

            cache.Add(account.Username, account);
            return account;
        }

        public virtual Task<Account> Update(Account account)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> Delete(string username)
        {
            throw new NotImplementedException();
        }
    }
}