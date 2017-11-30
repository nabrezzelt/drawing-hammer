using System;
using System.Runtime.Serialization;

namespace DrawingHammerServer.Exceptions
{
    [Serializable]
    public class UsernameTooShortException : Exception
    {
        public UsernameTooShortException()
        {
        }

        public UsernameTooShortException(string message) : base(message)
        {
        }

        public UsernameTooShortException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UsernameTooShortException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}