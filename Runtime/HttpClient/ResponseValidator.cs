using System;
using System.Net;
using SpicaSDK.Services.Exceptions;

namespace SpicaSDK
{
    public static class ResponseValidator
    {
        public static bool Validate(Response response)
        {
            if (!response.Success)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                throw new SpicaServerException("Request failed");
            }

            return true;
        }
    }
}