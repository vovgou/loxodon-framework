using System;

namespace Loxodon.Framework.Asynchronous
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPromise
    {
        /// <summary>
        /// The execution result
        /// </summary>
        object Result { get; }

        /// <summary>
        /// Exception
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Returns  "true" if this task finished.
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// Returns "true" if this task was cancelled before it completed normally.
        /// </summary>
        bool IsCancelled { get; }

        /// <summary>
        /// Returns  "true" if there is a cancellation request.
        /// </summary>
        bool IsCancellationRequested { get; }

        /// <summary>
        /// 
        /// </summary>
        void SetCancelled();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        void SetException(string error);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void SetException(Exception exception);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        void SetResult(object result = null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IPromise<TResult> : IPromise
    {
        /// <summary>
        /// The execution result
        /// </summary>
        new TResult Result { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        void SetResult(TResult result);
    }
}
