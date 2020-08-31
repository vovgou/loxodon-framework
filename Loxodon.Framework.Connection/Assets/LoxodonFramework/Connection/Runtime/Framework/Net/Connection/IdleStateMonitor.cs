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
            Task.Run(() =>
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
                                 TimeSpan waitTimeout = TimeSpan.FromTicks(Math.Min(Math.Min(readerIdleCheckTime, writerIdleCheckTime), allIdleCheckTime) - ticks);
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
             }, cancellationToken);
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

        public void OnShutdown()
        {
            Dispose(true);
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
