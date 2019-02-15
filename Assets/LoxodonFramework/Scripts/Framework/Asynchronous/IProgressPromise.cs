namespace Loxodon.Framework.Asynchronous
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProgress"></typeparam>
    public interface IProgressPromise<TProgress> : IPromise
    {
        /// <summary>
        /// The task's progress.
        /// </summary>
        TProgress Progress { get; }

        /// <summary>
        /// Update progress
        /// </summary>
        /// <param name="progress"></param>
        void UpdateProgress(TProgress progress);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProgress"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IProgressPromise<TProgress, TResult> : IProgressPromise<TProgress>, IPromise<TResult>
    {
    }
}
