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

using Loxodon.Framework.Messaging;
using System.Threading.Tasks;

namespace Loxodon.Framework.Examples
{
    public enum AccountEventType
    {
        Register,
        Update,
        Deleted,
        Login
    }
    public class AccountEventArgs
    {
        public AccountEventArgs(AccountEventType type, Account account)
        {
            this.Type = type;
            this.Account = account;
        }

        public AccountEventType Type { get; private set; }

        public Account Account { get; private set; }
    }

    public interface IAccountService
    {
        IMessenger Messenger { get; }

        Task<Account> Register(Account account);

        Task<Account> Update(Account account);

        Task<Account> Login(string username, string password);

        Task<Account> GetAccount(string username);
    }
}