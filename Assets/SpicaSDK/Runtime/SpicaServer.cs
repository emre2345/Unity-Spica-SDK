using UniRx;
using Cysharp.Threading.Tasks;
using SpicaSDK.Interfaces;
using SpicaSDK.Services.Models;
using SpicaSDK.Services.Services.Identity.Models;

namespace SpicaSDK.Services
{
    public class SpicaServer : ISpicaServer
    {
        private string rootUrl;

        private IHttpClient httpClient;

        public SpicaServer(ISpicaServerUrl rootUrl, IHttpClient httpClient)
        {
            this.rootUrl = rootUrl.RootUrl;
            this.httpClient = httpClient;
        }

        public async UniTask<Response> InitializeAsync()
        {
            var response = await httpClient.GetAsync(new Request(rootUrl));
            IsAvailable = response.Success;
            return response;
        }

        public bool IsAvailable { get; private set; }

        public string BucketUrl(Id bucketId) => $"{rootUrl}/api/bucket/{bucketId}";

        public string BucketDataUrl(Id bucketId) => $"{rootUrl}/api/bucket/{bucketId}/data";

        public string BucketDataDocumentUrl(Id bucketId, Id documentId) =>
            $"{rootUrl}/api/bucket/{bucketId}/data/{documentId}";

        public string FirehoseUrl => $"{rootUrl.Replace("http", "ws")}/api/firehose";

        public string IdentityUrl => $"{rootUrl}/api/passport/identify";

        private Identity identity;

        public Identity Identity
        {
            get => identity;
            set
            {
                identity = value;
                httpClient.AddDefaultHeader("Authorization", $"{identity.Scheme} {identity.Token}");
            }
        }
    }
}