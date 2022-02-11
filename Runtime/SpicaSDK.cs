using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Plugins.SpicaSDK.Runtime.Services.Function.Firehose;
using SpicaSDK.Interfaces;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using SpicaSDK.Services.Exceptions;
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
        private static Firehose firehose;

        static SpicaSDK()
        {
            httpClient = new HttpClient();
            webSocketClient = new WebSocketClient.WebSocketClient();
            spicaServer = new SpicaServer(SpicaServerConfiguration.Instance, httpClient);
            bucketService = new BucketService(spicaServer, httpClient, webSocketClient);
            firehose = new Firehose(spicaServer, webSocketClient);
        }

        public static bool LogEnabled => SpicaLogger.LogsEnabled;

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

        public static void SetJWT(string jwt)
        {
            spicaServer.Identity = new Identity(jwt, "IDENTITY", String.Empty);
        }

        public static class Bucket
        {
            public static UniTask<Models.Bucket[]> Get()
            {
                return bucketService.GetAllAsync();
            }

            public static class Realtime
            {
                public static UniTask<BucketConnection<T>> Connect<T>(Id bucketId, QueryParams queryParams)
                    where T : class
                {
                    return bucketService.Realtime.ConnectToBucketAsync<T>(bucketId, queryParams);
                }

                public static UniTask<DocumentChange<T>> WatchDocument<T>(Id bucketId, Id documentId)
                    where T : class
                {
                    return bucketService.Realtime.WatchDocumentAsync<T>(bucketId, documentId);
                }
            }

            public static class Data
            {
                public static UniTask<T> Insert<T>(Id bucketId, T document)
                {
                    return bucketService.Data.InsertAsync(bucketId, document);
                }

                public static UniTask<bool> Delete<T>(Id bucketId, Id documentId)
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
            }
        }

        public static class Http
        {
            public static UniTask<Response> Post(string functionName, string payload)
            {
                return httpClient.PostAsync(
                    new Request($"{SpicaServerConfiguration.Instance.RootUrl}/api/fn-execute/{functionName}", payload));
            }

            public static UniTask<Response> Get(string functionName, QueryParams queryParams)
            {
                return httpClient.GetAsync(
                    new Request($"{SpicaServerConfiguration.Instance.RootUrl}/api/fn-execute/{functionName}",
                        queryParams.GetString()));
            }
        }

        public static class Function
        {
            public static class Firehose
            {
                public static UniTask<FirehoseConnection<T>> Connect<T>(QueryParams queryParams)
                {
                    return firehose.Connect<T>(queryParams);
                }
            }
        }
    }
}