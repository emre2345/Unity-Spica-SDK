using System;
using System.Net;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using SpicaSDK.Services.Models;

namespace SpicaSDK.Services
{
    public partial class BucketService : ISpicaService
    {
        private ISpicaServer server;
        private IHttpClient httpClient;

        public BucketService(ISpicaServer server, IHttpClient httpClient)
        {
            this.server = server;
            this.httpClient = httpClient;
        }

        public async UniTask<Bucket> Get(string id)
        {
            var response = await httpClient.Get(new Request(server.BucketUrl(id)));

            if (!response.Success)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                throw new Exception("Get request failed");
            }

            return JsonConvert.DeserializeObject<Bucket>(response.Text);
        }
    }
}