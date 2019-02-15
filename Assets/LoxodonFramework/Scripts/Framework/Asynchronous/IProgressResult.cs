namespace Loxodon.Framework.Asynchronous
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProgress"></typeparam>
    public interface IProgressResult<TProgress> : IAsyncResult
    {
        /// <summary>
        /// The task's progress.
        /// </summary>
        TProgress Progress { get; }

        /// <summary>
        /// Gets a callbackable object.
        /// </summary>
        /// <returns></returns>
        new IProgressCallbackable<TProgress> Callbackable();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProgress"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IProgressResult<TProgress, TResult> : IAsyncResult<TResult>, IProgressResult<TProgress>
    {
        /// <summary>
        /// Gets a callbackable object.
        /// </summary>
        /// <returns></returns>
        new IProgressCallbackable<TProgress, TResult> Callbackable();
    }
}
