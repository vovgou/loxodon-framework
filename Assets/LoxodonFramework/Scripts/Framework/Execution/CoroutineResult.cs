using UnityEngine;
using System.Collections.Generic;

using Loxodon.Framework.Asynchronous;


namespace Loxodon.Framework.Execution
{
    public class CoroutineResult : AsyncResult, ICoroutinePromise
    {
        protected List<Coroutine> coroutines = new List<Coroutine>();

        public CoroutineResult() : base(true)
        {
        }

        public override bool Cancel()
        {
            if (this.IsDone)
                return false;

            this.cancellationRequested = true;
            foreach (Coroutine coroutine in this.coroutines)
            {
                Executors.StopCoroutine(coroutine);
            }
            this.SetCancelled();
            return true;
        }

        public void AddCoroutine(Coroutine coroutine)
        {
            this.coroutines.Add(coroutine);
        }
    }

    public class CoroutineResult<TResult> : AsyncResult<TResult>, ICoroutinePromise<TResult>
    {
        protected List<Coroutine> coroutines = new List<Coroutine>();

        public CoroutineResult() : base(true)
        {
        }

        public override bool Cancel()
        {
            if (this.IsDone)
                return false;

            this.cancellationRequested = true;
            foreach (Coroutine coroutine in this.coroutines)
            {
                Executors.StopCoroutine(coroutine);
            }
            this.SetCancelled();
            return true;
        }

        public void AddCoroutine(Coroutine coroutine)
        {
            this.coroutines.Add(coroutine);
        }
    }

    public class CoroutineProgressResult<TProgress> : ProgressResult<TProgress>, ICoroutineProgressPromise<TProgress>
    {
        protected List<Coroutine> coroutines = new List<Coroutine>();

        public CoroutineProgressResult() : base(true)
        {
        }

        public override bool Cancel()
        {
            if (this.IsDone)
                return false;

            this.cancellationRequested = true;
            foreach (Coroutine coroutine in this.coroutines)
            {
                Executors.StopCoroutine(coroutine);
            }
            this.SetCancelled();
            return true;
        }

        public void AddCoroutine(Coroutine coroutine)
        {
            this.coroutines.Add(coroutine);
        }
    }

    public class CoroutineProgressResult<TProgress, TResult> : ProgressResult<TProgress, TResult>, ICoroutineProgressPromise<TProgress, TResult>
    {
        protected List<Coroutine> coroutines = new List<Coroutine>();

        public CoroutineProgressResult() : base(true)
        {
        }

        public override bool Cancel()
        {
            if (this.IsDone)
                return false;

            this.cancellationRequested = true;
            foreach (Coroutine coroutine in this.coroutines)
            {
                Executors.StopCoroutine(coroutine);
            }
            this.SetCancelled();
            return true;
        }

        public void AddCoroutine(Coroutine coroutine)
        {
            this.coroutines.Add(coroutine);
        }
    }
}
