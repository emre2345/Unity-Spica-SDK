using System;
using System.Collections.Generic;
using System.Net;
using Cysharp.Threading.Tasks;
using SpicaSDK.Interfaces;
using SpicaSDK.Services.Exceptions;
using SpicaSDK.Services.Models;
using Newtonsoft.Json;
using UnityEngine;

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

        public async UniTask<Bucket> GetAsync(Id id)
        {
            var response = await httpClient.GetAsync(new Request(server.BucketUrl(id)));

            if (ResponseValidator.Validate(response))
            {
                return JsonConvert.DeserializeObject<Bucket>(response.Text);
            }

            throw new SpicaServerException();
        }

        public async UniTask<Bucket> CreateAsync(Bucket bucket)
        {
            var response =
                await httpClient.PostAsync(new Request(server.BucketUrl(new Id()), JsonConvert.SerializeObject(bucket)));

            if (ResponseValidator.Validate(response))
                return JsonConvert.DeserializeObject<Bucket>(response.Text);

            throw new SpicaServerException();
        }

        public async UniTask<HttpStatusCode> DeleteAsync(Id id)
        {
            var response =
                await httpClient.DeleteAsync(new Request(server.BucketUrl(id)));

            if (ResponseValidator.Validate(response))
                return response.StatusCode;

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

            public async UniTask<T> GetAsync<T>(Id bucketId, Id documentId, QueryParams queryParams)
            {
                var response = await httpClient.GetAsync(new Request(server.BucketDataDocumentUrl(bucketId, documentId),
                    queryParams.QueryString,
                    new Dictionary<string, string>(0)));

                if (ResponseValidator.Validate(response))
                    return JsonConvert.DeserializeObject<T>(response.Text);

                throw new SpicaServerException();
            }

            public async UniTask<T[]> GetAllAsync<T>(Id bucketId, QueryParams queryParams)
            {
                var response = await httpClient.GetAsync(new Request(server.BucketDataUrl(bucketId),
                    queryParams.QueryString,
                    new Dictionary<string, string>(0)));

                if (ResponseValidator.Validate(response))
                    return JsonConvert.DeserializeObject<T[]>(response.Text);

                throw new SpicaServerException();
            }

            public async UniTask<T> InsertAsync<T>(Id bucketId, T document)
            {
                var response = await httpClient.PostAsync(new Request(server.BucketDataUrl(bucketId),
                    JsonConvert.SerializeObject(document),
                    new Dictionary<string, string>(0)));

                if (ResponseValidator.Validate(response))
                    return JsonConvert.DeserializeObject<T>(response.Text);

                throw new SpicaServerException();
            }

            public async UniTask<T> PatchAsync<T>(Id bucketId, Id documentId, T document)
            {
                var response = await httpClient.PatchAsync(new Request(server.BucketDataDocumentUrl(bucketId, documentId),
                    JsonConvert.SerializeObject(document),
                    new Dictionary<string, string>(0)));

                if (ResponseValidator.Validate(response))
                    return document;

                throw new SpicaServerException();
            }

            public async UniTask<bool> RemoveAsync(Id bucketId, Id documentId)
            {
                var response = await httpClient.DeleteAsync(new Request(server.BucketDataDocumentUrl(bucketId, documentId),
                    string.Empty,
                    new Dictionary<string, string>(0)));

                if (ResponseValidator.Validate(response))
                    return true;

                throw new SpicaServerException();
            }

            public async UniTask<T> ReplaceAsync<T>(Id bucketId, Id documentId, T document)
            {
                var response = await httpClient.PutAsync(new Request(server.BucketDataDocumentUrl(bucketId, documentId),
                    JsonConvert.SerializeObject(document),
                    new Dictionary<string, string>(0)));

                if (ResponseValidator.Validate(response))
                    return JsonConvert.DeserializeObject<T>(response.Text);

                throw new SpicaServerException();
            }
        }
    }
}