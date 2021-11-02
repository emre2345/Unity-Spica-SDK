using UniRx;
using Cysharp.Threading.Tasks;
using SpicaSDK.Interfaces;
using SpicaSDK.Services.Services.Identity.Models;

namespace SpicaSDK.Services
{
    public class SpicaServer : ISpicaServer
    {
        private string rootUrl;

        private IHttpClient httpClient;

        public SpicaServer(string rootUrl, IHttpClient httpClient)
        {
            this.rootUrl = rootUrl;
            this.httpClient = httpClient;
        }

        public UniTask<bool> Initialize()
        {
            return UniTask.Create<bool>(async delegate
            {
                var response = await httpClient.Get(new Request(rootUrl));
                int code = (int)response.StatusCode;
                IsAvailable = code >= 200 && code < 300;
                return IsAvailable;
            });
        }

        public bool IsAvailable { get; private set; }

        public string BucketUrl(string bucketId) => $"{rootUrl}/api/bucket/{bucketId}?{Identity.Token}";

        public string BucketDataUrl(string bucketId) => $"{rootUrl}/api/bucket/{bucketId}/data?{Identity.Token}";

        public string IdentityUrl => $"{rootUrl}/passport/identify";

        public Identity Identity { get; set; }
    }
}