using System.Net;

namespace SpicaSDK
{
    public readonly struct Response
    {
        public readonly HttpStatusCode StatusCode;

        public readonly string Text;

        public bool Success => (int)StatusCode >= 200 && (int)StatusCode < 300;

        public Response(HttpStatusCode statusCode, string text)
        {
            StatusCode = statusCode;
            Text = text;
        }
    }
}