using System;

namespace Loxodon.Framework.Configurations
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException()
        {
        }

        public AlreadyExistsException(string message) : base(message)
        {
        }

        public AlreadyExistsException(Exception exception) : base("", exception)
        {
        }

        public AlreadyExistsException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
