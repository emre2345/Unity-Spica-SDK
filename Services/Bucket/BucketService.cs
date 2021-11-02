using System;
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

        public readonly DataService Data;

        public BucketService(ISpicaServer server, IHttpClient httpClient)
        {
            this.server = server;
            this.httpClient = httpClient;

            Data = new DataService(server, httpClient);
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

            public async UniTask<T> Get<T>(Id bucketId, Id documentId)
            {
                var response = await httpClient.Get(new Request(server.BucketDataDocumentUrl(bucketId, documentId)));

                if (ResponseValidator.Validate(response))
                    return JsonConvert.DeserializeObject<T>(response.Text);

                throw new SpicaServerException();
            }
        }

        public class RealtimeService : ISpicaService
        {
        }
    }
}