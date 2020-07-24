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

using System;
using System.Runtime.CompilerServices;

namespace Loxodon.Framework.Asynchronous
{
    public static class LuaTaskExtensions
    {
        public static IAwaiter GetAwaiter(this ILuaTask target)
        {
            return new LuaTaskAwaiter(target);
        }

        public static IAwaiter<T> GetAwaiter<T>(this ILuaTask<T> target)
        {
            return new LuaTaskAwaiter<T>(target);
        }

        public struct LuaTaskAwaiter : IAwaiter, ICriticalNotifyCompletion
        {
            private ILuaTask task;

            public LuaTaskAwaiter(ILuaTask task)
            {
                this.task = task ?? throw new ArgumentNullException("task");
            }

            public bool IsCompleted => task.IsCompleted;

            public void GetResult()
            {
                if (!IsCompleted)
                    throw new Exception("The task is not finished yet");

                if (task.GetException() != null)
                    throw new Exception(task.GetException());
            }

            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                if (continuation == null)
                    throw new ArgumentNullException("continuation");

                task.OnCompleted(continuation);
            }
        }

        public struct LuaTaskAwaiter<TResult> : IAwaiter<TResult>, ICriticalNotifyCompletion
        {
            private ILuaTask<TResult> task;

            public LuaTaskAwaiter(ILuaTask<TResult> task)
            {
                this.task = task ?? throw new ArgumentNullException("task");
            }

            public bool IsCompleted => task.IsCompleted;

            public TResult GetResult()
            {
                if (!IsCompleted)
                    throw new Exception("The task is not finished yet");

                if (task.GetException() != null)
                    throw new Exception(task.GetException());

                return task.GetResult();
            }

            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                if (continuation == null)
                    throw new ArgumentNullException("continuation");

                task.OnCompleted(continuation);
            }
        }
    }
}