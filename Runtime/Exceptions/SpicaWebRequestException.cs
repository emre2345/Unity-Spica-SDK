using System;
using Cysharp.Threading.Tasks;

namespace SpicaSDK.Services.Exceptions
{
    public class SpicaWebRequestException : SpicaServerException
    {
        public UnityWebRequestException WebException { get; }

        public SpicaWebRequestException(UnityWebRequestException innerException)
        {
            WebException = innerException;
        }
    }
}