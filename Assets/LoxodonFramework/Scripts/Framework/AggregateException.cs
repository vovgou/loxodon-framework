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
