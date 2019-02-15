using System;

namespace Loxodon.Framework.Asynchronous
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAsyncTask : IAsyncResult
    {
        /// <summary>
        /// Triggered when a task starts.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IAsyncTask OnPreExecute(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Triggered when the task is completed.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IAsyncTask OnPostExecute(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Triggered when an error occurs.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IAsyncTask OnError(Action<Exception> callback, bool runOnMainThread = true);

        /// <summary>
        /// Always call the end of the task.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IAsyncTask OnFinish(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Start the task after the given delay.
        /// </summary>
        /// <param name="delay">millisecond</param>
        /// <returns></returns>
        IAsyncTask Start(int delay);

        /// <summary>
        /// Start the task.
        /// </summary>
        /// <returns></returns>
        IAsyncTask Start();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IAsyncTask<TResult> : IAsyncResult<TResult>
    {
        /// <summary>
        /// Triggered when a task starts.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IAsyncTask<TResult> OnPreExecute(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Triggered when the task is completed.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IAsyncTask<TResult> OnPostExecute(Action<TResult> callback, bool runOnMainThread = true);

        /// <summary>
        /// Triggered when an error occurs.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IAsyncTask<TResult> OnError(Action<Exception> callback, bool runOnMainThread = true);

        /// <summary>
        /// Always call the end of the task.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IAsyncTask<TResult> OnFinish(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Start the task after the given delay.
        /// </summary>
        /// <param name="delay">millisecond</param>
        /// <returns></returns>
        IAsyncTask<TResult> Start(int delay);

        /// <summary>
        /// Start the task.
        /// </summary>
        /// <returns></returns>
        IAsyncTask<TResult> Start();
    }

}
