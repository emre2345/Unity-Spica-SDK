using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SpicaSDK.Interfaces;
using SpicaSDK.Services.Exceptions;
using SpicaSDK.Services.Models;
using Newtonsoft.Json;

namespace SpicaSDK.Services
{
    public partial class BucketService : ISpicaService
    {
        private ISpicaServer server;
        private IHttpClient httpClient;
        private IWebSocketClient webSocketClient;

        public readonly DataService Data;

        public BucketService(ISpicaServer server, IHttpClient httpClient, IWebSocketClient webSocketClient)
        {
            this.server = server;
            this.httpClient = httpClient;

            Data = new DataService(server, httpClient);
            Realtime = new RealtimeService(server, webSocketClient);
        }

        public async UniTask<Bucket> Get(Id id)
        {
            var response = await httpClient.Get(new Request(server.BucketUrl(id)));

            if (ResponseValidator.Validate(response))
            {
                return JsonConvert.DeserializeObject<Bucket>(response.Text);
            }

            throw new SpicaServerException();
        }

        public class DataService : ISpicaService
        {
            private ISpicaServer server;
            private IHttpClient httpClient;

            public DataService(ISpicaServer server, IHttpClient httpClient)
            {
                this.server = server;
                this.httpClient = httpClient;
            }

            public async UniTask<T> Get<T>(Id bucketId, Id documentId, QueryParams queryParams)
            {
                var response = await httpClient.Get(new Request(server.BucketDataDocumentUrl(bucketId, documentId),
                    queryParams.QueryString,
                    new Dictionary<string, string>(0)));

                if (ResponseValidator.Validate(response))
                    return JsonConvert.DeserializeObject<T>(response.Text);

                throw new SpicaServerException();
            }

            public async UniTask<T[]> GetAll<T>(Id bucketId, QueryParams queryParams)
            {
                var response = await httpClient.Get(new Request(server.BucketDataUrl(bucketId),
                    queryParams.QueryString,
                    new Dictionary<string, string>(0)));

                if (ResponseValidator.Validate(response))
                    return JsonConvert.DeserializeObject<T[]>(response.Text);

                throw new SpicaServerException();
            }

            public async UniTask<T> Insert<T>(Id bucketId, T document)
            {
                var response = await httpClient.Post(new Request(server.BucketDataUrl(bucketId),
                    JsonConvert.SerializeObject(document),
                    new Dictionary<string, string>(0)));

                if (ResponseValidator.Validate(response))
                    return JsonConvert.DeserializeObject<T>(response.Text);

                throw new SpicaServerException();
            }

            public async UniTask<T> Patch<T>(Id bucketId, Id documentId, T document)
            {
                var response = await httpClient.Patch(new Request(server.BucketDataDocumentUrl(bucketId, documentId),
                    JsonConvert.SerializeObject(document),
                    new Dictionary<string, string>(0)));

                if (ResponseValidator.Validate(response))
                    return JsonConvert.DeserializeObject<T>(response.Text);

                throw new SpicaServerException();
            }

            public async UniTask<bool> Remove(Id bucketId, Id documentId)
            {
                var response = await httpClient.Delete(new Request(server.BucketDataDocumentUrl(bucketId, documentId),
                    string.Empty,
                    new Dictionary<string, string>(0)));

                if (ResponseValidator.Validate(response))
                    return true;

                throw new SpicaServerException();
            }
        }
    }
}