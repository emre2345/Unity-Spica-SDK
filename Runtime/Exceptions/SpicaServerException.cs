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
    }
}