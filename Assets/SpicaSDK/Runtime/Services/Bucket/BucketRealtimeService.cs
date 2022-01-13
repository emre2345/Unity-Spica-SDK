using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using SpicaSDK.Runtime.Utils;
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

            public async UniTask<DocumentChange<T>> WatchDocumentAsync<T>(Id bucketId, Id documentId) where T : class
            {
                MongoAggregation mongoAggregation = new MongoAggregation(1);
                mongoAggregation.Add("_id", documentId.Value);

                QueryParams queryParams = new QueryParams(1);
                queryParams.Add("filter", mongoAggregation.GetString());
                queryParams.Add("Authorization", $"{server.Identity.Scheme} {server.Identity.Token}");

                string url = server.BucketDataUrl(bucketId).Replace("http", "ws") + "?" + queryParams.GetString();
                IWebSocketConnection connection = await webSocketClient.ConnectAsync(url);
                return new DocumentChange<T>(connection);
            }

            public async UniTask<BucketConnection<T>> ConnectToBucketAsync<T>(Id bucketId, QueryParams queryParams)
                where T : class
            {
                queryParams.Add("Authorization", $"{server.Identity.Scheme} {server.Identity.Token}");
                string url = server.BucketDataUrl(bucketId).Replace("http", "ws") + "?" + queryParams.GetString();
                var connection = await webSocketClient.ConnectAsync(url);
                return new BucketConnection<T>(connection);
            }
        }
    }
}