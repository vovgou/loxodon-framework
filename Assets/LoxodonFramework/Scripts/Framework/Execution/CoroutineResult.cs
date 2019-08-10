/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

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
