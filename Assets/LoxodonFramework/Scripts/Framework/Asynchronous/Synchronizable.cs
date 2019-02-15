using System;
using System.Threading;

using Loxodon.Log;

namespace Loxodon.Framework.Asynchronous
{
    public interface ISynchronizable
    {
        /// <summary>
        ///  Wait for done,will block the current thread.
        /// </summary>
        /// <returns></returns>
        bool WaitForDone();

        /// <summary>
        /// Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        object WaitForResult(int millisecondsTimeout = 0);

        /// <summary>
        ///  Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="timeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        object WaitForResult(TimeSpan timeout);
    }

    public interface ISynchronizable<TResult> : ISynchronizable
    {
        /// <summary>
        /// Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        new TResult WaitForResult(int millisecondsTimeout = 0);

        /// <summary>
        /// Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="timeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        new TResult WaitForResult(TimeSpan timeout);
    }

    internal class Synchronizable : ISynchronizable
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(Synchronizable));

        private IAsyncResult result;
        private object _lock;
        public Synchronizable(IAsyncResult result, object _lock)
        {
            this.result = result;
            this._lock = _lock;
        }

        /// <summary>
        /// Wait for done,will block the current thread.
        /// </summary>
        /// <returns></returns>
        public bool WaitForDone()
        {
            if (result.IsDone)
                return result.IsDone;

            lock (_lock)
            {
                if (!result.IsDone)
                    Monitor.Wait(_lock);
            }

            return result.IsDone;
        }

        /// <summary>
        /// Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public object WaitForResult(int millisecondsTimeout = 0)
        {
            if (result.IsDone)
            {
                if (result.Exception != null)
                    throw result.Exception;

                return result.Result;
            }

            lock (_lock)
            {
                if (!result.IsDone)
                {
                    if (millisecondsTimeout > 0)
                        Monitor.Wait(_lock, millisecondsTimeout);
                    else
                        Monitor.Wait(_lock);
                }
            }

            if (!result.IsDone)
                throw new TimeoutException();

            if (result.Exception != null)
                throw result.Exception;

            return result.Result;
        }

        /// <summary>
        ///  Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="timeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public object WaitForResult(TimeSpan timeout)
        {
            if (result.IsDone)
            {
                if (result.Exception != null)
                    throw result.Exception;

                return result.Result;
            }

            lock (_lock)
            {
                if (!result.IsDone)
                {
                    Monitor.Wait(_lock, timeout);
                }
            }

            if (!result.IsDone)
                throw new TimeoutException();

            if (result.Exception != null)
                throw result.Exception;

            return result.Result;
        }
    }

    internal class Synchronizable<TResult> : ISynchronizable<TResult>
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(Synchronizable<TResult>));

        private IAsyncResult<TResult> result;
        private object _lock;
        public Synchronizable(IAsyncResult<TResult> result, object _lock)
        {
            this.result = result;
            this._lock = _lock;
        }

        /// <summary>
        /// Wait for done,will block the current thread.
        /// </summary>
        /// <returns></returns>
        public bool WaitForDone()
        {
            if (result.IsDone)
                return result.IsDone;

            lock (_lock)
            {
                if (!result.IsDone)
                    Monitor.Wait(_lock);
            }

            return result.IsDone;
        }

        /// <summary>
        /// Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public TResult WaitForResult(int millisecondsTimeout = 0)
        {
            if (result.IsDone)
            {
                if (result.Exception != null)
                    throw result.Exception;

                return result.Result;
            }

            lock (_lock)
            {
                if (!result.IsDone)
                {
                    if (millisecondsTimeout > 0)
                        Monitor.Wait(_lock, millisecondsTimeout);
                    else
                        Monitor.Wait(_lock);
                }
            }

            if (!result.IsDone)
                throw new TimeoutException();

            if (result.Exception != null)
                throw result.Exception;

            return result.Result;
        }

        /// <summary>
        /// Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="timeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public TResult WaitForResult(TimeSpan timeout)
        {
            if (result.IsDone)
            {
                if (result.Exception != null)
                    throw result.Exception;

                return result.Result;
            }

            lock (_lock)
            {
                if (!result.IsDone)
                {
                    Monitor.Wait(_lock, timeout);
                }
            }

            if (!result.IsDone)
                throw new TimeoutException();

            if (result.Exception != null)
                throw result.Exception;

            return result.Result;
        }

        object ISynchronizable.WaitForResult(int millisecondsTimeout)
        {
            return WaitForResult(millisecondsTimeout);
        }

        object ISynchronizable.WaitForResult(TimeSpan timeout)
        {
            return WaitForResult(timeout);
        }
    }
}
