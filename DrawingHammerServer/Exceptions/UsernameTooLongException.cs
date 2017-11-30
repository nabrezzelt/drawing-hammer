using System;
using System.Runtime.Serialization;

namespace DrawingHammerServer.Exceptions
{
    [Serializable]
    public class UsernameTooLongException : Exception
    {
        public UsernameTooLongException()
        {
        }

        public UsernameTooLongException(string message) : base(message)
        {
        }

        public UsernameTooLongException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UsernameTooLongException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}