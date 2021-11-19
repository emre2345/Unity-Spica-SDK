using System;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using SpicaSDK.Services.Models;
using SpicaSDK.Services.WebSocketClient;
using UniRx;

namespace SpicaSDK.Services
{
    public partial class BucketService
    {
        public readonly RealtimeService Realtime;

        public class RealtimeService : ISpicaService
        {
            private ISpicaServer server;
            private IWebSocketClient webSocketClient;

            public RealtimeService(ISpicaServer server, IWebSocketClient webSocketClient)
            {
                this.server = server;
                this.webSocketClient = webSocketClient;
            }

            public DocumentChange<T> WatchDocument<T>(Id bucketId, Id documentId) where T : class
            {
                QueryParams queryParams = new QueryParams(1);
                queryParams.AddQuery("filter", $"_id=={documentId.Value}");

                string url = server.BucketDataUrl(bucketId).Replace("http", "ws") + "&" + queryParams.QueryString;
                IObservable<Message> connection = webSocketClient.Connect(url);
                return new DocumentChange<T>(connection);
            }

            public BucketConnection<T> ConnectToBucket<T>(Id bucketId, QueryParams queryParams) where T : class
            {
                string url = server.BucketDataUrl(bucketId).Replace("http", "ws") + "&" + queryParams.QueryString;
                return new BucketConnection<T>(webSocketClient.Connect(url));
            }
        }
    }
}