using System;

namespace Loxodon.Framework.Views
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(Exception exception) : base("", exception)
        {
        }

        public NotFoundException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
