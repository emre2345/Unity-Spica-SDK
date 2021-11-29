using Cysharp.Threading.Tasks;
using SpicaSDK.Interfaces;
using SpicaSDK.Services.Services.Identity;
using SpicaSDK.Services.Services.Identity.Models;

namespace SpicaSDK.Services
{
    public static class SpicaSDK
    {
        private static ISpicaServer spicaServer;
        private static IHttpClient httpClient;

        static SpicaSDK()
        {
            httpClient = new HttpClient();
            spicaServer = new SpicaServer(SpicaServerConfiguration.Instance, httpClient);
        }

        public static void SetIdentity(Identity identity)
        {
            spicaServer.Identity = identity;
        }

        public static UniTask<Identity> LogIn(string username, string password)
        {
            IdentityService identityService = new IdentityService(spicaServer, httpClient);
            return identityService.LogInAsync(username, password, float.MaxValue);
        }
    }
}