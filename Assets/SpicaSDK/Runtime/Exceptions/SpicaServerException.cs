using System;

namespace SpicaSDK.Services.Exceptions
{
    public class SpicaServerException : Exception
    {
        public SpicaServerException()
        {
        }

        public SpicaServerException(string message) : base(message)
        {
        }

        public SpicaServerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}