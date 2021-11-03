using System.Net;

namespace SpicaSDK.Services.WebSocketClient
{
    public readonly struct Message
    {
        public readonly DataChangeType ChangeType;

        public readonly HttpStatusCode StatusCode;

        public readonly string Text;

        public Message(DataChangeType changeType, HttpStatusCode statusCode, string text)
        {
            ChangeType = changeType;
            StatusCode = statusCode;
            Text = text;
        }
    }
}