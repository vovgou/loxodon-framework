using System;

namespace Loxodon.Framework.Asynchronous
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProgress"></typeparam>
    public interface IProgressTask<TProgress> : IProgressResult<TProgress>
    {
        /// <summary>
        /// Triggered when a task starts.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress> OnPreExecute(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Triggered when the task is completed.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress> OnPostExecute(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Triggered when an error occurs.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress> OnError(Action<Exception> callback, bool runOnMainThread = true);

        /// <summary>
        /// Always call the end of the task.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress> OnFinish(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Update the progress.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress> OnProgressUpdate(Action<TProgress> callback, bool runOnMainThread = true);

        /// <summary>
        /// Start the task after the given delay.
        /// </summary>
        /// <param name="delay">millisecond</param>
        /// <returns></returns>
        IProgressTask<TProgress> Start(int delay);

        /// <summary>
        /// Start the task.
        /// </summary>
        /// <returns></returns>
        IProgressTask<TProgress> Start();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProgress"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IProgressTask<TProgress, TResult> : IProgressResult<TProgress, TResult>
    {
        /// <summary>
        /// Triggered when a task starts.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress, TResult> OnPreExecute(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Triggered when the task is completed.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress, TResult> OnPostExecute(Action<TResult> callback, bool runOnMainThread = true);

        /// <summary>
        /// Triggered when an error occurs.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress, TResult> OnError(Action<Exception> callback, bool runOnMainThread = true);

        /// <summary>
        /// Always call the end of the task.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress, TResult> OnFinish(Action callback, bool runOnMainThread = true);

        /// <summary>
        /// Update the progress.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="runOnMainThread"></param>
        /// <returns></returns>
        IProgressTask<TProgress, TResult> OnProgressUpdate(Action<TProgress> callback, bool runOnMainThread = true);

        /// <summary>
        /// Start the task after the given delay.
        /// </summary>
        /// <param name="delay">millisecond</param>
        /// <returns></returns>
        IProgressTask<TProgress, TResult> Start(int delay);

        /// <summary>
        /// Start the task.
        /// </summary>
        /// <returns></returns>
        IProgressTask<TProgress, TResult> Start();
    }
}
