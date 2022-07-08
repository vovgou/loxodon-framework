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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Loxodon.Framework.Net.Connection
{
    public class DefaultConnector<TRequest, TResponse, TNotification> : IConnector<TRequest, TResponse, TNotification> where TRequest : IRequest where TResponse : IResponse where TNotification : INotification
    {
        protected const int DEFAULT_TIMEOUT = 5000;

        protected readonly SemaphoreSlim connectLock = new SemaphoreSlim(1, 1);
        protected readonly Subject<EventArgs> eventArgsSubject = new Subject<EventArgs>();
        protected readonly Subject<TNotification> notificationSubject = new Subject<TNotification>();
        protected readonly ConcurrentDictionary<IRequest, RequestTaskTimeoutOrCompletionSource> promises = new ConcurrentDictionary<IRequest, RequestTaskTimeoutOrCompletionSource>();
        protected readonly ConcurrentStack<IMessage> messages = new ConcurrentStack<IMessage>();
        protected string hostname;
        protected int port;
        protected int connTimeoutMilliseconds;
        protected int timeoutMilliseconds;

        protected CancellationTokenSource readTokenSource;
        protected CancellationTokenSource shutdownTokenSource;
        protected CancellationToken shutdownCancellationToken;
        protected readonly object stateLock = new object();
        protected readonly object readLock = new object();
        protected ConnectionState state = ConnectionState.Closed;
        protected IChannel<IMessage> channel;
        protected IdleStateMonitor idleStateMonitor;
        protected IHandshakeHandler handshakeHandler;
        public DefaultConnector(IChannel<IMessage> channel) : this(channel, null, null)
        {
        }

        public DefaultConnector(IChannel<IMessage> channel, IdleStateMonitor idleStateMonitor) : this(channel, idleStateMonitor, null)
        {
        }

        public DefaultConnector(IChannel<IMessage> channel, IHandshakeHandler handshakeHandler) : this(channel, null, handshakeHandler)
        {
        }

        public DefaultConnector(IChannel<IMessage> channel, IdleStateMonitor idleStateMonitor, IHandshakeHandler handshakeHandler)
        {
            this.channel = channel ?? throw new ArgumentNullException(nameof(channel));
            this.handshakeHandler = handshakeHandler;
            this.idleStateMonitor = idleStateMonitor ?? new IdleStateMonitor(TimeSpan.FromSeconds(30.0));
            this.idleStateMonitor.IdleStateChanged += OnIdleStateChanged;
        }

        public DefaultConnector(IChannelFactory channelFactory, IHandshakeHandler handshakeHandler) : this(channelFactory, null, handshakeHandler)
        {
        }

        public DefaultConnector(IChannelFactory channelFactory, IdleStateMonitor idleStateMonitor) : this(channelFactory, idleStateMonitor, null)
        {
        }

        public DefaultConnector(IChannelFactory channelFactory, IdleStateMonitor idleStateMonitor, IHandshakeHandler handshakeHandler)
        {
            this.channel = channelFactory.CreateChannel();
            this.handshakeHandler = handshakeHandler;
            this.idleStateMonitor = idleStateMonitor ?? new IdleStateMonitor(TimeSpan.FromSeconds(30.0));
            this.idleStateMonitor.IdleStateChanged += OnIdleStateChanged;

        }

        public virtual int TimeoutMilliseconds
        {
            get { return this.timeoutMilliseconds; }
            set { this.timeoutMilliseconds = Math.Max(value, DEFAULT_TIMEOUT); }
        }

        public virtual bool Connected
        {
            get
            {
                if (this.channel == null || !this.channel.Connected)
                    return false;

                if (State == ConnectionState.Connected)
                    return true;

                return false;
            }
        }

        public virtual bool AutoReconnect { get; set; }

        public virtual ConnectionState State
        {
            get { return this.state; }
            protected set
            {
                lock (stateLock)
                {
                    if (this.state == value)
                        return;

                    ConnectionState oldState = this.state;
                    this.state = value;
                    this.OnStateChanged(oldState, value);
                }
            }
        }

        protected virtual void OnStateChanged(ConnectionState oldState, ConnectionState newState)
        {
        }

        protected virtual void Init()
        {
            if (shutdownTokenSource == null)
            {
                shutdownTokenSource = new CancellationTokenSource();
                shutdownCancellationToken = shutdownTokenSource.Token;
                if (timeoutMilliseconds <= 0)
                    timeoutMilliseconds = DEFAULT_TIMEOUT;

                Task.Factory.StartNew(DoTick, shutdownCancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        protected virtual void OnIdleStateChanged(object sender, IdleStateEventArgs e)
        {
            if (eventArgsSubject != null)
                eventArgsSubject.Publish(e);
        }

        public IChannel<IMessage> Channel { get { return this.channel; } }

        public Task Connect(string hostname, int port, int timeoutMilliseconds)
        {
            return Connect(hostname, port, timeoutMilliseconds, CancellationToken.None);
        }

        public async Task Connect(string hostname, int port, int timeoutMilliseconds, CancellationToken cancellationToken)
        {
            ValidateDisposed();
            if (timeoutMilliseconds <= 0)
                timeoutMilliseconds = DEFAULT_TIMEOUT;

            if (!await connectLock.WaitAsync(timeoutMilliseconds, cancellationToken).ConfigureAwait(false))
                throw new TimeoutException();

            CancellationTokenSource connectionTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            try
            {
                this.Init();

                await this.DoDisconnect().ConfigureAwait(false);
                this.State = ConnectionState.Connecting;
                this.eventArgsSubject.Publish(ConnectionEventArgs.ConnectingEventArgs);
                this.hostname = hostname;
                this.port = port;
                this.connTimeoutMilliseconds = timeoutMilliseconds;
                connectionTokenSource.CancelAfter(connTimeoutMilliseconds);
                CancellationToken connectionToken = connectionTokenSource.Token;
                connectionToken.Register(() =>
                {
                    if (channel != null && State == ConnectionState.Connecting)
                        channel.Close();
                });

                await DoConnect(connectionToken).ConfigureAwait(false);
                this.State = ConnectionState.Connected;
                connectionTokenSource.Dispose();
                this.eventArgsSubject.Publish(ConnectionEventArgs.ConnectedEventArgs);
            }
            catch (Exception)
            {
                await DoDisconnect().ConfigureAwait(false);
                this.State = ConnectionState.Exception;
                this.eventArgsSubject.Publish(ConnectionEventArgs.FailedEventArgs);
                throw;
            }
            finally
            {
                if (connectionTokenSource != null)
                    connectionTokenSource.Dispose();
                connectLock.Release();
            }
        }

        public Task Reconnect()
        {
            return Reconnect(false, CancellationToken.None);
        }

        public Task Reconnect(CancellationToken cancellationToken)
        {
            return Reconnect(false, cancellationToken);
        }

        protected async Task Reconnect(bool autoReconnect, CancellationToken cancellationToken)
        {
            await connectLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            CancellationTokenSource connectionTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            try
            {
                await this.DoDisconnect().ConfigureAwait(false);
                this.State = ConnectionState.Connecting;
                this.eventArgsSubject.Publish(ConnectionEventArgs.ReconnectingEventArgs);
                connectionTokenSource.CancelAfter(this.connTimeoutMilliseconds);
                CancellationToken connectionToken = connectionTokenSource.Token;
                connectionToken.Register(() =>
                {
                    if (channel != null && State == ConnectionState.Connecting)
                        channel.Close();
                });
                await DoConnect(connectionToken).ConfigureAwait(false);
                this.State = ConnectionState.Connected;
                connectionTokenSource.Dispose();
                this.eventArgsSubject.Publish(ConnectionEventArgs.ConnectedEventArgs);
            }
            catch (Exception)
            {
                await DoDisconnect().ConfigureAwait(false);
                this.State = ConnectionState.Exception;
                this.eventArgsSubject.Publish(autoReconnect ? ConnectionEventArgs.ExceptionEventArgs : ConnectionEventArgs.FailedEventArgs);
                throw;
            }
            finally
            {
                if (connectionTokenSource != null)
                    connectionTokenSource.Dispose();
                connectLock.Release();
            }
        }

        public virtual async Task Disconnect()
        {
            if (State == ConnectionState.Closed)
                return;

            await connectLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (State == ConnectionState.Closed)
                    return;

                this.State = ConnectionState.Closing;
                this.eventArgsSubject.Publish(ConnectionEventArgs.ClosingEventArgs);
                await DoDisconnect().ConfigureAwait(false);
                this.State = ConnectionState.Closed;
                this.eventArgsSubject.Publish(ConnectionEventArgs.ClosedEventArgs);
            }
            finally
            {
                connectLock.Release();
            }
        }

        public virtual async Task Shutdown()
        {
            await connectLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (shutdownTokenSource == null)
                    return;

                this.shutdownTokenSource.Cancel();
                this.shutdownTokenSource = null;

                if (State != ConnectionState.Closed)
                {
                    this.State = ConnectionState.Closing;
                    this.eventArgsSubject.Publish(ConnectionEventArgs.ClosingEventArgs);
                    await DoDisconnect().ConfigureAwait(false);
                    this.State = ConnectionState.Closed;
                    this.eventArgsSubject.Publish(ConnectionEventArgs.ClosedEventArgs);
                }

                if (idleStateMonitor != null)
                {
                    idleStateMonitor.IdleStateChanged -= OnIdleStateChanged;
                    idleStateMonitor.Dispose();
                }

                foreach (var kv in promises)
                {
                    var promise = kv.Value;
                    promise.SetCanceled();
                }
                this.promises.Clear();
            }
            catch (Exception) { }
            finally
            {
                connectLock.Release();
            }
        }

        public ISubscription<EventArgs> Events()
        {
            return this.eventArgsSubject.Subscribe();
        }

        public ISubscription<TNotification> Received()
        {
            return this.notificationSubject.Subscribe();
        }

        public ISubscription<TNotification> Received(Predicate<TNotification> filter)
        {
            return this.notificationSubject.Subscribe(filter);
        }

        public Task<TResponse> Send(TRequest request)
        {
            return Send(request, this.TimeoutMilliseconds);
        }

        public Task<TResponse> Send(TRequest request, int timeoutMilliseconds)
        {
            return Send(request, timeoutMilliseconds, CancellationToken.None);
        }

        public Task<TResponse> Send(TRequest request, CancellationToken cancellationToken)
        {
            return Send(request, TimeoutMilliseconds, cancellationToken);
        }

        public virtual async Task<TResponse> Send(TRequest request, int timeoutMilliseconds, CancellationToken cancellationToken)
        {
            ValidateDisposed();
            ValidateConnected();
            return (TResponse)await DoSend(request, timeoutMilliseconds, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task Send(TNotification notification)
        {
            ValidateDisposed();
            ValidateConnected();
            await DoSend(notification).ConfigureAwait(false);
        }

        protected virtual async Task DoConnect(CancellationToken cancellationToken)
        {
            try
            {
                await channel.Connect(hostname, port, connTimeoutMilliseconds, cancellationToken).ConfigureAwait(false);
                await DoHandshake(channel, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                OnConnected();
                cancellationToken.ThrowIfCancellationRequested();

                this.messages.Clear();
                this.readTokenSource = new CancellationTokenSource();
                _ = Task.Factory.StartNew(DoReceived, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                this.Read();
            }catch(Exception e)
            {
                if (this.readTokenSource != null)
                {
                    this.readTokenSource.Cancel();
                    this.readTokenSource = null;
                }
                throw e;
            }
        }

        protected virtual async Task DoHandshake(IChannel<IMessage> channel, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (handshakeHandler != null)
                await handshakeHandler.OnHandshake(channel).ConfigureAwait(false);
        }

        protected virtual void OnConnected()
        {
            idleStateMonitor?.OnConnected();
        }

        protected virtual async Task DoDisconnect()
        {
            try
            {
                if (this.channel != null)
                    await channel.Close().ConfigureAwait(false);

                if (readTokenSource != null)
                {
                    readTokenSource.Cancel();
                    readTokenSource = null;
                    lock (readLock)
                    {
                        Monitor.PulseAll(readLock);
                    }
                }

                OnDisconnected();
            }
            catch (Exception) { }
        }

        protected virtual void OnDisconnected()
        {
            idleStateMonitor?.OnDisconnected();
        }

        protected void DoReceived()
        {
            CancellationToken token = this.readTokenSource.Token;
            while (true)
            {
                IMessage message;
                if (messages.TryPop(out message))
                {
                    OnReceived(message);
                    continue;
                }

                lock (readLock)
                {
                    if (token.IsCancellationRequested && messages.Count <= 0)
                        break;

                    Monitor.Wait(readLock, 2000);
                }
            }
        }

        protected virtual Task<IResponse> DoSend(IRequest request, int timeoutMilliseconds, CancellationToken cancellationToken)
        {
            int timeout = Math.Max(timeoutMilliseconds, DEFAULT_TIMEOUT);
            RequestTaskTimeoutOrCompletionSource promise = new RequestTaskTimeoutOrCompletionSource(request, timeout, cancellationToken);
            promises.TryAdd(promise.Request, promise);
            this.channel.WriteAsync(request).ContinueWith(t =>
            {
                if (!t.IsFaulted && !t.IsCanceled)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        promise.TrySetCanceled();
                        promises.TryRemove(promise.Request, out _);
                    }

                    OnSent(request);
                }
                else
                {
                    if (t.Exception != null)
                        promise.TrySetException(t.Exception);
                    else
                        promise.TrySetException(new IOException());

                    promises.TryRemove(promise.Request, out _);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return promise.Task;
        }

        protected virtual async Task DoSend(TNotification notification)
        {
            await this.channel.WriteAsync(notification).ConfigureAwait(false);
            OnSent(notification);
        }

        protected virtual void OnSent(IMessage message)
        {
            idleStateMonitor?.OnSent();
        }

        protected void Read()
        {
            if (this.channel == null || !this.channel.Connected)
                return;

            this.channel.ReadAsync().ContinueWith(async (t) =>
            {
                if (!t.IsFaulted && !t.IsCanceled)
                {
                    //try
                    //{
                    //    this.OnReceived(t.Result);
                    //}
                    //finally
                    //{
                    //    Read();
                    //}

                    messages.Push(t.Result);
                    Read();

                    lock (readLock)
                    {
                        Monitor.PulseAll(readLock);
                    }
                }
                else
                {
                    //重连
                    if (this.State != ConnectionState.Connected)
                        return;

                    OnIOException(t.Exception);
                    await DoDisconnect();
                    if (this.State != ConnectionState.Connected)
                        return;

                    if (!AutoReconnect)
                    {
                        this.State = ConnectionState.Exception;
                        this.eventArgsSubject.Publish(ConnectionEventArgs.ExceptionEventArgs);
                        return;
                    }
                    await Reconnect(true, CancellationToken.None);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        protected virtual void OnReceived(IMessage message)
        {
            idleStateMonitor?.OnReceived();
            if (message is TNotification notification)
            {
                this.notificationSubject.Publish(notification);
                return;
            }

            if (message is IResponse response)
            {
                foreach (var request in promises.Keys)
                {
                    if (request.Sequence == response.Sequence)
                    {
                        RequestTaskTimeoutOrCompletionSource promise;
                        if (promises.TryRemove(request, out promise) && promise != null)
                            promise.TrySetResult(response);
                        break;
                    }
                }
                return;
            }
        }

        protected virtual void OnIOException(Exception e)
        {
        }

        protected virtual async void DoTick()
        {
            while (!this.shutdownCancellationToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var kv in promises)
                    {
                        var request = kv.Key;
                        var promise = kv.Value;

                        if (promise.IsCanceled)
                        {
                            promise.TrySetCanceled();
                            promises.TryRemove(request, out _);
                        }

                        if (promise.IsTimeout)
                        {
                            promise.TrySetTimeout();
                            promises.TryRemove(request, out _);
                        }
                    }
                }
                catch (Exception) { }

                try
                {
                    await Task.Delay(timeoutMilliseconds / 2, shutdownCancellationToken).ConfigureAwait(false);
                }
                catch (Exception) { }
            }
        }

        protected virtual void ValidateConnected()
        {
            if (!this.Connected)
                throw new IOException("The connection is not established or disconnected.");
        }

        protected virtual void ValidateDisposed()
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(this.GetType().FullName);
        }

        #region IEqualityComparer<uint> Support
        public class UInt32EqualityComparer : IEqualityComparer<uint>
        {
            bool IEqualityComparer<uint>.Equals(uint x, uint y)
            {
                return x == y;
            }

            int IEqualityComparer<uint>.GetHashCode(uint obj)
            {
                return obj.GetHashCode();
            }
        }
        #endregion

        #region RequestTaskTimeoutOrCompletionSource Support
        protected class RequestTaskTimeoutOrCompletionSource : TaskTimeoutOrCompletionSource<IResponse>
        {
            private IRequest request;
            public RequestTaskTimeoutOrCompletionSource(IRequest request, int timeoutMilliseconds, CancellationToken cancellationToken) : base(timeoutMilliseconds, cancellationToken)
            {
                this.request = request;
            }

            public IRequest Request { get { return this.request; } }

            public uint Sequence { get { return this.request.Sequence; } }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false;

#pragma warning disable 4014
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (this.shutdownTokenSource != null)
                {
                    try
                    {
                        shutdownTokenSource.Cancel();
                        this.shutdownTokenSource = null;

                        if (this.channel != null)
                        {
                            this.State = ConnectionState.Closing;
                            DoDisconnect();
                            this.channel = null;
                        }

                        if (idleStateMonitor != null)
                        {
                            idleStateMonitor.IdleStateChanged -= OnIdleStateChanged;
                            idleStateMonitor.Dispose();
                            idleStateMonitor = null;
                        }

                        this.eventArgsSubject.Dispose();
                        this.notificationSubject.Dispose();
                        foreach (var kv in promises)
                        {
                            var promise = kv.Value;
                            promise.SetCanceled();
                        }
                        this.promises.Clear();
                    }
                    finally
                    {
                        this.State = ConnectionState.Closed;
                    }
                }
                disposedValue = true;
            }
        }

        ~DefaultConnector()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
