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

namespace Loxodon.Framework.Asynchronous
{
    public class ImmutableAsyncResult : AsyncResult
    {
        public ImmutableAsyncResult() : base(false)
        {
            this.SetResult(null);
        }

        public ImmutableAsyncResult(object result) : base(false)
        {
            this.SetResult(result);
        }

        public ImmutableAsyncResult(Exception exception) : base(false)
        {
            this.SetException(exception);
        }
    }

    public class ImmutableAsyncResult<TResult> : AsyncResult<TResult>
    {
        public ImmutableAsyncResult() : base(false)
        {
            this.SetResult(default(TResult));
        }

        public ImmutableAsyncResult(TResult result) : base(false)
        {
            this.SetResult(result);
        }

        public ImmutableAsyncResult(Exception exception) : base(false)
        {
            this.SetException(exception);
        }
    }
}
