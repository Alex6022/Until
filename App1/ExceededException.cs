using System;

namespace App1
{
    internal class ExceededException : Exception
    {
        public ExceededException()
        {
        }

        public ExceededException(string message) : base(message)
        {
        }

        public ExceededException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}