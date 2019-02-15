using System;

namespace Loxodon.Framework.Asynchronous
{
    /// <summary>
    /// IAsyncResult
    /// </summary>
    public interface IAsyncResult
    {
        /// <summary>
        /// Gets the result of the asynchronous operation.
        /// </summary>
        object Result { get; }

        /// <summary>
        /// Gets the cause of the asynchronous operation.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Returns <code>true</code> if the asynchronous operation is finished.
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// Returns <code>true</code> if the asynchronous operation was cancelled before it completed normally.
        /// </summary>
        bool IsCancelled { get; }

        /// <summary>
        /// Attempts to cancel execution of this task.  This attempt will 
        /// fail if the task has already completed, has already been cancelled,
        /// or could not be cancelled for some other reason.If successful,
        /// and this task has not started when "Cancel" is called,
        /// this task should never run. 
        /// </summary>
        /// <exception cref="NotSupportedException">If not supported, throw an exception.</exception>
        /// <returns></returns>
        bool Cancel();

        /// <summary>
        /// Gets a callbackable object.
        /// </summary>
        /// <returns></returns>
        ICallbackable Callbackable();

        /// <summary>
        /// Gets a synchronized object.
        /// </summary>
        /// <returns></returns>
        ISynchronizable Synchronized();

        /// <summary>
        /// Wait for the result,suspends the coroutine.
        /// eg:
        /// IAsyncResult result;
        /// yiled return result.WaitForDone();
        /// </summary>
        /// <returns></returns>
        object WaitForDone();

    }

    /// <summary>
    /// IAsyncResult
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IAsyncResult<TResult> : IAsyncResult
    {
        /// <summary>
        /// Gets the result of the asynchronous operation.
        /// </summary>
        new TResult Result { get; }

        /// <summary>
        /// Gets a callbackable object.
        /// </summary>
        /// <returns></returns>
        new ICallbackable<TResult> Callbackable();

        /// <summary>
        /// Gets a synchronized object.
        /// </summary>
        /// <returns></returns>
        new ISynchronizable<TResult> Synchronized();
    }
}
