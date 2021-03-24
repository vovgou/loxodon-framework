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

using System.Threading;
using System.Threading.Tasks;

namespace Loxodon.Framework.Net.Connection
{
    public interface IChannel<T>
    {
        /// <summary>
        /// Whether a connection has been established.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Connects the Client to the specified port on the specified host.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <returns></returns>
        Task Connect(string hostname, int port, int timeoutMilliseconds);

        /// <summary>
        /// Connects the Client to the specified port on the specified host.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Connect(string hostname, int port, int timeoutMilliseconds, CancellationToken cancellationToken);

        /// <summary>
        /// Read a message asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<T> ReadAsync();

        /// <summary>
        /// Write a message asynchronously.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task WriteAsync(T message);

        /// <summary>
        ///  Forces a channel to close.
        /// </summary>
        /// <returns></returns>
        Task Close();
    }
}
