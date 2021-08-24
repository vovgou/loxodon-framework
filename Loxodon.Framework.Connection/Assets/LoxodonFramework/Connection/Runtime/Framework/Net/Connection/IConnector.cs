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
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Loxodon.Framework.Net.Connection
{
    public interface IConnector<TRequest, TResponse, TNotification> : IDisposable where TRequest : IRequest where TResponse : IResponse where TNotification : INotification
    {
        /// <summary>
        /// Whether a connection has been established.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Whether to automatically reconnect to the server when the connection is abnormal, 
        /// it only tries to reconnect once, if the connection to the server fails, it will not try again
        /// </summary> 
        bool AutoReconnect { get; set; }        

        /// <summary>
        /// Connects the Client to the specified port on the specified host.
        /// Supports IPV6 and IPV4, it is recommended to use domain names instead of IP addresses
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <returns></returns>
        Task Connect(string hostname, int port, int timeoutMilliseconds);

        /// <summary>
        /// Connects the Client to the specified port on the specified host.
        /// Supports IPV6 and IPV4, it is recommended to use domain names instead of IP addresses
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Connect(string hostname, int port, int timeoutMilliseconds, CancellationToken cancellationToken);

        /// <summary>
        /// Reconnect to the server
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <returns></returns>
        Task Reconnect();

        /// <summary>
        /// Reconnect to the server
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Reconnect(CancellationToken cancellationToken);

        /// <summary>
        /// Forces a connection to disconnect.
        /// </summary>
        /// <returns></returns>
        Task Disconnect();

        /// <summary>
        /// Shutdown service
        /// </summary>
        /// <returns></returns>
        Task Shutdown();

        /// <summary>
        /// Subscribe to events
        /// </summary>
        /// <returns></returns>
        ISubscription<EventArgs> Events();

        /// <summary>
        /// Subscribe to notification messages
        /// </summary>
        /// <returns></returns>
        ISubscription<TNotification> Received();

        /// <summary>
        /// Subscribe to notification messages
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        ISubscription<TNotification> Received(Predicate<TNotification> filter);

        /// <summary>
        /// Send a request message
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        Task<TResponse> Send(TRequest request);

        /// <summary>
        /// Send a request message
        /// </summary>
        /// <param name="request"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        Task<TResponse> Send(TRequest request, int timeoutMilliseconds);

        /// <summary>
        /// Send a request message
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        Task<TResponse> Send(TRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Send a request message
        /// </summary>
        /// <param name="request"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        Task<TResponse> Send(TRequest request, int timeoutMilliseconds, CancellationToken cancellationToken);

        /// <summary>
        /// Send a notification message
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        Task Send(TNotification notification);

    }
}
