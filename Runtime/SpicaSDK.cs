using Cysharp.Threading.Tasks;
using SpicaSDK.Interfaces;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Services.Models;
using SpicaSDK.Services.Services.Identity;
using SpicaSDK.Services.Services.Identity.Models;

namespace SpicaSDK.Services
{
    public static class SpicaSDK
    {
        private static ISpicaServer spicaServer;
        private static IHttpClient httpClient;
        private static IWebSocketClient webSocketClient;
        private static BucketService bucketService;

        static SpicaSDK()
        {
            httpClient = new HttpClient();
            webSocketClient = new WebSocketClient.WebSocketClient();
            spicaServer = new SpicaServer(SpicaServerConfiguration.Instance, httpClient);
            bucketService = new BucketService(spicaServer, httpClient, webSocketClient);
        }

        public static bool LoggedIn { get; private set; }

        public static void SetIdentity(Identity identity)
        {
            spicaServer.Identity = identity;

            LoggedIn = true;
        }

        public static UniTask<Identity> LogIn(string username, string password)
        {
            IdentityService identityService = new IdentityService(spicaServer, httpClient);
            return identityService.LogInAsync(username, password, float.MaxValue);
        }

        public static void SetApiKey(string apiKey)
        {
            spicaServer.Identity = new Identity(apiKey, "APIKEY", string.Empty);
        }

        public static UniTask<Bucket[]> GetBuckets()
        {
            return bucketService.GetAllAsync();
        }

        public static UniTask<BucketConnection<T>> ConnectToBucket<T>(Id bucketId, QueryParams queryParams)
            where T : class
        {
            return bucketService.Realtime.ConnectToBucketAsync<T>(bucketId, queryParams);
        }

        public static UniTask<DocumentChange<T>> WatchDocument<T>(Id bucketId, Id documentId)
            where T : class
        {
            return bucketService.Realtime.WatchDocumentAsync<T>(bucketId, documentId);
        }

        public static UniTask<T> InsertAsync<T>(Id bucketId, T document)
        {
            return bucketService.Data.InsertAsync(bucketId, document);
        }

        public static UniTask<bool> DeleteAsync<T>(Id bucketId, Id documentId)
        {
            return bucketService.Data.RemoveAsync(bucketId, documentId);
        }

        public static UniTask<T> Replace<T>(Id bucketId, Id documentId, T document)
        {
            return bucketService.Data.ReplaceAsync(bucketId, documentId, document);
        }

        public static UniTask<T> Patch<T>(Id bucketId, Id documentId, T document)
        {
            return bucketService.Data.PatchAsync(bucketId, documentId, document);
        }

        public static UniTask<T> Get<T>(Id bucketId, Id documentId, QueryParams queryParams)
        {
            return bucketService.Data.GetAsync<T>(bucketId, documentId, queryParams);
        }

        public static UniTask<T[]> GetAll<T>(Id bucketId, QueryParams queryParams)
        {
            return bucketService.Data.GetAllAsync<T>(bucketId, queryParams);
        }

        public static UniTask<Response> Post(string functionName, string payload)
        {
            return httpClient.PostAsync(
                new Request($"{SpicaServerConfiguration.Instance.RootUrl}/api/fn-execute/{functionName}", payload));
        }
    }
}