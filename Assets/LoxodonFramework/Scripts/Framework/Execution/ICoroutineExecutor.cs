using System;
using System.Collections;

using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Execution
{
    public interface ICoroutineExecutor
    {
        void RunOnCoroutineNoReturn(IEnumerator routine);

        Asynchronous.IAsyncResult RunOnCoroutine(IEnumerator routine);

        Asynchronous.IAsyncResult RunOnCoroutine(Func<IPromise, IEnumerator> func);

        IAsyncResult<TResult> RunOnCoroutine<TResult>(Func<IPromise<TResult>, IEnumerator> func);

        IProgressResult<TProgress> RunOnCoroutine<TProgress>(Func<IProgressPromise<TProgress>, IEnumerator> func);

        IProgressResult<TProgress, TResult> RunOnCoroutine<TProgress, TResult>(Func<IProgressPromise<TProgress, TResult>, IEnumerator> func);
    }
}
