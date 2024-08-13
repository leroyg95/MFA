#region

using System;

#endregion

namespace MultiFactorAuthentication.Exceptions
{
    public class SmsException : Exception
    {
        public SmsException()
        {
        }

        public SmsException(string message) : base(message)
        {
        }

        public SmsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}