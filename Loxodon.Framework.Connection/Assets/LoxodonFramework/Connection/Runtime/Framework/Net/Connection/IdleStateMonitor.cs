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
using System.Threading;
using System.Threading.Tasks;

namespace Loxodon.Framework.Net.Connection
{
    public class IdleStateMonitor : IDisposable
    {
        private readonly object syncLock = new object();
        private TimeSpan readerIdleTime;
        private TimeSpan writerIdleTime;
        private TimeSpan allIdleTime;
        private TimeSpan waitTimeout;

        private long readerIdleCheckTime;
        private long writerIdleCheckTime;
        private long allIdleCheckTime;

        private bool enableReaderIdle;
        private bool enableWriterIdle;
        private bool enableAllIdle;

        private bool firstReaderIdle = true;
        private bool firstWriterIdle = true;
        private bool firstAllIdle = true;

        private bool connected = false;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        public IdleStateMonitor() : this(TimeSpan.FromMilliseconds(0))
        {
        }

        public IdleStateMonitor(TimeSpan idleTime) : this(idleTime, idleTime, idleTime)
        {
        }

        public IdleStateMonitor(TimeSpan readerIdleTime, TimeSpan writerIdleTime, TimeSpan allIdleTime)
        {
            this.readerIdleTime = readerIdleTime;
            this.writerIdleTime = writerIdleTime;
            this.allIdleTime = allIdleTime;

            this.enableReaderIdle = this.readerIdleTime.Ticks > 0;
            this.enableWriterIdle = this.writerIdleTime.Ticks > 0;
            this.enableAllIdle = this.allIdleTime.Ticks > 0;

            waitTimeout = TimeSpan.FromSeconds(60);
            if (enableReaderIdle && waitTimeout > readerIdleTime)
                waitTimeout = readerIdleTime;

            if (enableWriterIdle && waitTimeout > writerIdleTime)
                waitTimeout = writerIdleTime;

            if (enableAllIdle && waitTimeout > allIdleTime)
                waitTimeout = allIdleTime;

            Init();
        }


        public event EventHandler<IdleStateEventArgs> IdleStateChanged;

        protected void Init()
        {
            if (!(enableReaderIdle || enableWriterIdle || enableAllIdle))
                return;

            long t = DateTime.Now.Ticks;
            readerIdleCheckTime = t + readerIdleTime.Ticks;
            writerIdleCheckTime = t + writerIdleTime.Ticks;
            allIdleCheckTime = t + allIdleTime.Ticks;
            firstReaderIdle = firstWriterIdle = firstAllIdle = true;

            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = cancellationTokenSource.Token;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    while (true)
                    {
                        if (this.cancellationToken.IsCancellationRequested)
                            break;

                        long ticks = DateTime.Now.Ticks;
                        if (connected)
                        {
                            try
                            {
                                IdleStateEventArgs readerIdleEventArgs = null;
                                IdleStateEventArgs writerIdleEventArgs = null;
                                IdleStateEventArgs allIdleEventArgs = null;

                                lock (syncLock)
                                {
                                    if (enableReaderIdle && readerIdleCheckTime <= ticks)
                                    {
                                        readerIdleEventArgs = firstReaderIdle ? IdleStateEventArgs.FirstReaderIdleStateEvent : IdleStateEventArgs.ReaderIdleStateEvent;
                                        firstReaderIdle = false;
                                        readerIdleCheckTime = ticks + readerIdleTime.Ticks;
                                    }

                                    if (enableWriterIdle && writerIdleCheckTime <= ticks)
                                    {
                                        writerIdleEventArgs = firstWriterIdle ? IdleStateEventArgs.FirstWriterIdleStateEvent : IdleStateEventArgs.WriterIdleStateEvent;
                                        firstWriterIdle = false;
                                        writerIdleCheckTime = ticks + writerIdleTime.Ticks;
                                    }

                                    if (enableAllIdle && allIdleCheckTime <= ticks)
                                    {
                                        allIdleEventArgs = firstAllIdle ? IdleStateEventArgs.FirstAllIdleStateEvent : IdleStateEventArgs.AllIdleStateEvent;
                                        firstAllIdle = false;
                                        allIdleCheckTime = ticks + allIdleTime.Ticks;
                                    }
                                }

                                if (readerIdleEventArgs != null)
                                    RaiseIdleStateChanged(readerIdleEventArgs);
                                if (writerIdleEventArgs != null)
                                    RaiseIdleStateChanged(writerIdleEventArgs);
                                if (allIdleEventArgs != null)
                                    RaiseIdleStateChanged(allIdleEventArgs);
                            }
                            catch (Exception) { }
                        }

                        lock (syncLock)
                        {
                            if (connected)
                            {
                                if (waitTimeout.Ticks > 0)
                                    Monitor.Wait(syncLock, waitTimeout);
                            }
                            else
                            {
                                Monitor.Wait(syncLock);
                            }
                        }
                    }
                }
                catch (Exception) { }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            //Task.Run(() =>
            // {
            //     try
            //     {
            //         while (true)
            //         {
            //             if (this.cancellationToken.IsCancellationRequested)
            //                 break;

            //             long ticks = DateTime.Now.Ticks;
            //             if (connected)
            //             {
            //                 try
            //                 {
            //                     IdleStateEventArgs readerIdleEventArgs = null;
            //                     IdleStateEventArgs writerIdleEventArgs = null;
            //                     IdleStateEventArgs allIdleEventArgs = null;

            //                     lock (syncLock)
            //                     {
            //                         if (enableReaderIdle && readerIdleCheckTime <= ticks)
            //                         {
            //                             readerIdleEventArgs = firstReaderIdle ? IdleStateEventArgs.FirstReaderIdleStateEvent : IdleStateEventArgs.ReaderIdleStateEvent;
            //                             firstReaderIdle = false;
            //                             readerIdleCheckTime = ticks + readerIdleTime.Ticks;
            //                         }

            //                         if (enableWriterIdle && writerIdleCheckTime <= ticks)
            //                         {
            //                             writerIdleEventArgs = firstWriterIdle ? IdleStateEventArgs.FirstWriterIdleStateEvent : IdleStateEventArgs.WriterIdleStateEvent;
            //                             firstWriterIdle = false;
            //                             writerIdleCheckTime = ticks + writerIdleTime.Ticks;
            //                         }

            //                         if (enableAllIdle && allIdleCheckTime <= ticks)
            //                         {
            //                             allIdleEventArgs = firstAllIdle ? IdleStateEventArgs.FirstAllIdleStateEvent : IdleStateEventArgs.AllIdleStateEvent;
            //                             firstAllIdle = false;
            //                             allIdleCheckTime = ticks + allIdleTime.Ticks;
            //                         }
            //                     }

            //                     if (readerIdleEventArgs != null)
            //                         RaiseIdleStateChanged(readerIdleEventArgs);
            //                     if (writerIdleEventArgs != null)
            //                         RaiseIdleStateChanged(writerIdleEventArgs);
            //                     if (allIdleEventArgs != null)
            //                         RaiseIdleStateChanged(allIdleEventArgs);
            //                 }
            //                 catch (Exception) { }
            //             }

            //             lock (syncLock)
            //             {
            //                 if (connected)
            //                 {
            //                     if (waitTimeout.Ticks > 0)
            //                         Monitor.Wait(syncLock, waitTimeout);
            //                 }
            //                 else
            //                 {
            //                     Monitor.Wait(syncLock);
            //                 }
            //             }
            //         }
            //     }
            //     catch (Exception) { }
            // }, cancellationToken);
        }

        public void OnConnected()
        {
            lock (syncLock)
            {
                connected = true;
                Monitor.PulseAll(syncLock);
            }
        }

        public void OnReceived()
        {
            lock (syncLock)
            {
                long ticks = DateTime.Now.Ticks;
                if (enableReaderIdle)
                    readerIdleCheckTime = ticks + readerIdleTime.Ticks;
                if (enableAllIdle)
                    allIdleCheckTime = ticks + allIdleTime.Ticks;
                firstReaderIdle = firstAllIdle = true;
            }
        }

        public void OnSent()
        {
            lock (syncLock)
            {
                long ticks = DateTime.Now.Ticks;
                if (enableWriterIdle)
                    writerIdleCheckTime = ticks + writerIdleTime.Ticks;
                if (enableAllIdle)
                    allIdleCheckTime = ticks + allIdleTime.Ticks;
                firstWriterIdle = firstAllIdle = true;
            }
        }

        public void OnDisconnected()
        {
            lock (syncLock)
            {
                connected = false;
                Monitor.PulseAll(syncLock);
            }
        }

        protected void RaiseIdleStateChanged(IdleStateEventArgs eventArgs)
        {
            IdleStateChanged?.Invoke(this, eventArgs);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                lock (syncLock)
                {
                    connected = false;
                    this.cancellationTokenSource.Cancel();
                    Monitor.PulseAll(syncLock);
                    this.cancellationTokenSource.Dispose();
                    this.cancellationTokenSource = null;
                }
                disposedValue = true;
            }
        }

        ~IdleStateMonitor()
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
