using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using SpicaSDK.Services.Services.Identity.Models;

namespace SpicaSDK.Services.Services.Identity
{
    public class IdentityService : ISpicaService
    {
        private ISpicaServer server;
        private IHttpClient httpClient;

        public IdentityService(ISpicaServer server, IHttpClient httpClient)
        {
            this.server = server;
            this.httpClient = httpClient;
        }

        public async UniTask<Models.Identity> LogIn(string identity, string password, float lifespan)
        {
            var payload = new Dictionary<string, object>(3);
            payload.Add("identity", identity);
            payload.Add("password", password);
            payload.Add("expires", lifespan);

            var response = await httpClient.Post(new Request(server.IdentityUrl, JsonConvert.SerializeObject(payload)));

            if (!response.Success)
                throw new Exception("Identity request failed");

            return JsonConvert.DeserializeObject<Models.Identity>(response.Text);
        }
    }
}