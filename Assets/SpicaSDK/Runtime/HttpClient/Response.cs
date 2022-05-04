using System.Collections.Generic;
using System.Net;

namespace SpicaSDK
{
    public readonly struct Response
    {
        public readonly HttpStatusCode StatusCode;

        public readonly string Text;

        public readonly Dictionary<string, string> Headers;

        public bool Success => (int)StatusCode >= 200 && (int)StatusCode < 300;

        public Response(HttpStatusCode statusCode, string text, Dictionary<string, string> headers)
        {
            StatusCode = statusCode;
            Text = text;
            Headers = headers;
        }

        public override string ToString()
        {
            return $"[ {nameof(Response)} ]\n{StatusCode}\n{Text}";
        }
    }
}