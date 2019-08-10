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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Loxodon.Framework
{
    public class AggregateException : Exception
    {
        private ReadOnlyCollection<Exception> innerExceptions;

        public AggregateException(IList<Exception> innerExceptions) : this("", innerExceptions)
        {
        }

        public AggregateException(string message, IList<Exception> innerExceptions) : base(message)
        {
            if (innerExceptions == null)
                throw new ArgumentNullException("innerExceptions");

            List<Exception> list = new List<Exception>();
            for (int i = 0; i < innerExceptions.Count; i++)
            {
                var exception = innerExceptions[i];
                if (exception == null)
                    continue;
                list.Add(exception);
            }
            this.innerExceptions = list.AsReadOnly();
        }

        public ReadOnlyCollection<Exception> InnerExceptions
        {
            get { return this.innerExceptions; }
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(base.ToString()).Append(Environment.NewLine);
            for (int i = 0; i < innerExceptions.Count; i++)
            {
                buf.Append(Environment.NewLine)
                    .Append(innerExceptions[i].ToString())
                    .Append(Environment.NewLine);
            }
            return buf.ToString();
        }
    }
}
