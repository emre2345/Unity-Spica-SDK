using System;
using Cysharp.Threading.Tasks;
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

            public async UniTask<DocumentChange<T>> WatchDocument<T>(Id bucketId, Id documentId) where T : class
            {
                QueryParams queryParams = new QueryParams(1);
                queryParams.AddQuery("filter", $"_id==\"{documentId.Value}\"");
                queryParams.AddQuery("Authorization", $"{server.Identity.Scheme} {server.Identity.Token}");

                string url = server.BucketDataUrl(bucketId).Replace("http", "ws") + "?" + queryParams.QueryString;
                IWebSocketConnection connection = await webSocketClient.ConnectAsync(url);
                return new DocumentChange<T>(connection);
            }

            public async UniTask<BucketConnection<T>> ConnectToBucket<T>(Id bucketId, QueryParams queryParams)
                where T : class
            {
                queryParams.AddQuery("Authorization", $"{server.Identity.Scheme} {server.Identity.Token}");
                string url = server.BucketDataUrl(bucketId).Replace("http", "ws") + "?" + queryParams.QueryString;
                var connection = await webSocketClient.ConnectAsync(url);
                return new BucketConnection<T>(connection);
            }
        }
    }
}