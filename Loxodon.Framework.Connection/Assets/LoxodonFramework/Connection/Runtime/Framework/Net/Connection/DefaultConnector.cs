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
        protected readonly ConcurrentDictionary<string, TaskTimeoutOrCompletionSource<TResponse>> promises = new ConcurrentDictionary<string, TaskTimeoutOrCompletionSource<TResponse>>();

        protected string hostname;
        protected int port;
        protected int connTimeoutMilliseconds;
        protected int timeoutMilliseconds;

        protected bool running = false;
        protected object stateLock = new object();
        protected ConnectionState state = ConnectionState.Closed;
        protected IChannel<IMessage> channel;

        public DefaultConnector(IChannel<IMessage> channel)
        {
            this.channel = channel ?? throw new ArgumentNullException("channel");
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

                    this.state = value;
                    Monitor.PulseAll(stateLock);
                }
            }
        }

        protected virtual void Init()
        {
            if (!running)
            {
                running = true;
                if (timeoutMilliseconds <= 0)
                    timeoutMilliseconds = DEFAULT_TIMEOUT;
                Task.Run(DoReceived);
                Task.Run(DoTick);
            }
        }

        public async Task Connect(string hostname, int port, int timeoutMilliseconds)
        {
            ValidateDisposed();
            await connectLock.WaitAsync();
            try
            {
                this.Init();

                await this.DoDisconnect();
                this.State = ConnectionState.Connecting;
                this.eventArgsSubject.Publish(ConnectionEventArgs.ConnectingEventArgs);
                this.hostname = hostname;
                this.port = port;
                this.connTimeoutMilliseconds = timeoutMilliseconds;
                //await DoConnect().TimeoutAfter(this.connTimeoutMilliseconds);
                await DoConnect();
                this.State = ConnectionState.Connected;
                this.eventArgsSubject.Publish(ConnectionEventArgs.ConnectedEventArgs);
            }
            catch (Exception)
            {
                try
                {
                    await DoDisconnect();
                }
                catch (Exception) { }
                this.State = ConnectionState.Exception;
                this.eventArgsSubject.Publish(ConnectionEventArgs.FailedEventArgs);
                throw;
            }
            finally
            {
                connectLock.Release();
            }
        }

        public async Task Reconnect()
        {
            await connectLock.WaitAsync();
            try
            {
                await this.DoDisconnect();

                this.State = ConnectionState.Connecting;
                this.eventArgsSubject.Publish(ConnectionEventArgs.ReconnectingEventArgs);
                await DoConnect().TimeoutAfter(this.connTimeoutMilliseconds);
                this.State = ConnectionState.Connected;
                this.eventArgsSubject.Publish(ConnectionEventArgs.ConnectedEventArgs);
            }
            catch (Exception)
            {
                await DoDisconnect();
                this.State = ConnectionState.Exception;
                this.eventArgsSubject.Publish(ConnectionEventArgs.FailedEventArgs);
                throw;
            }
            finally
            {
                connectLock.Release();
            }
        }

        public async Task Disconnect()
        {
            await connectLock.WaitAsync();
            if (State == ConnectionState.Closed)
                return;

            try
            {
                this.State = ConnectionState.Closing;
                this.eventArgsSubject.Publish(ConnectionEventArgs.ClosingEventArgs);
                await DoDisconnect();
            }
            finally
            {
                this.State = ConnectionState.Closed;
                connectLock.Release();
                this.eventArgsSubject.Publish(ConnectionEventArgs.ClosedEventArgs);
            }
        }

        public async Task Shutdown()
        {
            await connectLock.WaitAsync();
            try
            {
                if (!running)
                    return;

                this.running = false;
                lock (stateLock)
                {
                    Monitor.PulseAll(stateLock);
                }

                if (State != ConnectionState.Closed)
                {
                    this.State = ConnectionState.Closing;
                    this.eventArgsSubject.Publish(ConnectionEventArgs.ClosingEventArgs);
                    await DoDisconnect();
                    this.State = ConnectionState.Closed;
                    this.eventArgsSubject.Publish(ConnectionEventArgs.ClosedEventArgs);
                }

                foreach (var kv in promises)
                {
                    var promise = kv.Value;
                    promise.SetCanceled();
                }
                this.promises.Clear();
            }
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

        public virtual async Task<TResponse> Send(TRequest request, int timeoutMilliseconds)
        {
            ValidateDisposed();
            ValidateConnected();
            return await this.DoSend(request, timeoutMilliseconds);
        }

        public virtual async Task Send(TNotification notification)
        {
            ValidateDisposed();
            ValidateConnected();
            await this.DoSend(notification);
        }

        protected virtual async Task DoConnect()
        {
            await channel.Connect(hostname, port, connTimeoutMilliseconds);
        }

        protected virtual async Task DoDisconnect()
        {
            if (this.channel != null && this.channel.Connected)
            {
                await channel.Close();
            }
        }

        protected virtual Task<TResponse> DoSend(TRequest request, int timeoutMilliseconds)
        {
            int timeout = Math.Max(timeoutMilliseconds, DEFAULT_TIMEOUT);
            TaskTimeoutOrCompletionSource<TResponse> promise = new TaskTimeoutOrCompletionSource<TResponse>(timeout);
            this.channel.WriteAsync(request).ContinueWith(t =>
            {
                if (t.IsCompleted)
                {
                    promises.TryAdd(request.Sequence.ToString(), promise);
                }
                else
                {
                    if (t.Exception != null)
                        promise.TrySetException(t.Exception);
                    else
                        promise.TrySetException(new IOException());
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return promise.Task;
        }

        protected virtual Task DoSend(TNotification notification)
        {
            return this.channel.WriteAsync(notification);
        }

        protected virtual async void DoReceived()
        {
            while (running)
            {
                try
                {
                    lock (stateLock)
                    {
                        if (!this.Connected)
                        {
                            Monitor.Wait(stateLock);
                            continue;
                        }
                    }

                    IMessage message = await this.channel.ReadAsync();
                    if (message is TNotification)
                    {
                        this.notificationSubject.Publish((TNotification)message);
                        continue;
                    }

                    if (message is TResponse)
                    {
                        TResponse response = (TResponse)message;
                        TaskTimeoutOrCompletionSource<TResponse> promise;
                        if (promises.TryRemove(response.Sequence.ToString(), out promise) && promise != null)
                            promise.SetResult(response);
                        continue;
                    }
                }
                catch (Exception)
                {
                    if (this.State == ConnectionState.Connected)
                    {
                        this.eventArgsSubject.Publish(ConnectionEventArgs.ExceptionEventArgs);
                        await DoDisconnect();
                        if (this.State != ConnectionState.Connected)
                            return;

                        if (AutoReconnect)
                        {
                            try
                            {
                                await Reconnect();
                            }
                            catch (Exception) { }
                        }
                        else
                        {
                            this.State = ConnectionState.Exception;
                        }
                    }
                }
            }
        }

        protected virtual void DoTick()
        {
            while (running)
            {
                try
                {
                    foreach (var kv in promises)
                    {
                        var sequence = kv.Key;
                        var promise = kv.Value;
                        if (promise.IsTimeout)
                        {
                            promise.SetTimeout();
                            promises.TryRemove(sequence, out _);
                        }
                    }
                }
                catch (Exception) { }

                lock (stateLock)
                {
                    int count = promises.Count;
                    if (count <= 0 && (!State.Equals(ConnectionState.Connected)))
                        Monitor.Wait(stateLock);
                    else
                        Monitor.Wait(stateLock, timeoutMilliseconds / 2);
                }
            }
        }

        protected virtual void ValidateConnected()
        {
            if (!this.Connected)
                throw new IOException("Connection not established.");
        }

        protected virtual void ValidateDisposed()
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(this.GetType().FullName);
        }

        #region IDisposable Support
        private bool disposedValue = false;

#pragma warning disable 4014
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (running)
                {
                    try
                    {
                        this.running = false;
                        lock (stateLock)
                        {
                            Monitor.PulseAll(stateLock);
                        }
                        if (this.channel != null)
                        {
                            this.State = ConnectionState.Closing;
                            DoDisconnect();
                            this.channel = null;
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
