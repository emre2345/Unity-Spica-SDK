using Cysharp.Threading.Tasks;
using SpicaSDK.Interfaces;
using SpicaSDK.Runtime.Services.Bucket.Realtime;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using SpicaSDK.Services.Models;
using SpicaSDK.Services.WebSocketClient;

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
                QueryParams queryParams = new QueryParams(1);
                queryParams.Add("Authorization", $"{server.Identity.Scheme} {server.Identity.Token}");

                if (documentId.Equals(Id.Empty))
                {
                    MongoAggregation mongoAggregation = new MongoAggregation(1);
                    mongoAggregation.Add("_id", documentId.Value);
                    queryParams.Add("filter", mongoAggregation.GetString());
                }


                string url = server.BucketDataUrl(bucketId).Replace("http", "ws") + "?" + queryParams.GetString();
                IBucketRealtimeConnection connection =
                    await webSocketClient.ConnectAsync(url, socket => new BucketRealtimeConnection(socket)) as IBucketRealtimeConnection;
                return new DocumentChange<T>(connection);
            }

            public async UniTask<BucketConnection<T>> ConnectToBucketAsync<T>(Id bucketId, QueryParams queryParams)
                where T : class
            {
                queryParams.Add("Authorization", $"{server.Identity.Scheme} {server.Identity.Token}");
                string url = server.BucketDataUrl(bucketId).Replace("http", "ws") + "?" + queryParams.GetString();
                var connection = await webSocketClient.ConnectAsync(url, socket => new BucketRealtimeConnection(socket)) as IBucketRealtimeConnection;
                return new BucketConnection<T>(connection);
            }
        }
    }
}