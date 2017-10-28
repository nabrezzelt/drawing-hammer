using System;
using System.Runtime.Serialization;

namespace DrawingHammerServer.Exceptions
{
    public class UserAlreadyExitsException : Exception
    {
        public UserAlreadyExitsException()
        {
        }

        public UserAlreadyExitsException(string message) : base(message)
        {
        }

        public UserAlreadyExitsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserAlreadyExitsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}