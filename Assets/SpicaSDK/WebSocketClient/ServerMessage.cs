using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SpicaSDK.Services.WebSocketClient
{
    public readonly struct ServerMessage
    {
        public readonly DataChangeType Kind;

        public readonly JObject Document;

        public readonly string Message;

        public readonly int Status;

        [JsonConstructor]
        public ServerMessage(int kind, JObject document, int status = 200, string message = "") : this(
            (DataChangeType)kind, document, status,
            message)
        {
        }

        public ServerMessage(DataChangeType kind, JObject document, int status, string message)
        {
            Kind = kind;
            Document = document;
            Message = message;
            Status = status;
        }

        public ServerMessage(DataChangeType kind, string document)
        {
            Kind = kind;
            Document = JObject.Parse(document);
            Message = string.Empty;
            Status = 200;
        }


        public override string ToString()
        {
            return $"[ {nameof(Message)} ] - {JsonConvert.SerializeObject(this)}";
        }
    }
}