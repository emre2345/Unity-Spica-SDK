using System.Collections;
using System.Net;
using Cysharp.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SpicaSDK.Interfaces;
using SpicaSDK.Services.Services.Identity;
using SpicaSDK.Services.Services.Identity.Models;
using UnityEngine.TestTools;

namespace SpicaSDK.Tests.Editor.Unit
{
    public class IdentityServiceTests
    {
        private string identity = "testIndentity";
        private string password = "testPassword";
        private float lifespan = float.MaxValue;

        [UnityTest]
        public IEnumerator LogsIn() => UniTask.ToCoroutine(async delegate
        {
            ISpicaServer server = Substitute.For<ISpicaServer>();
            IHttpClient client = Substitute.For<IHttpClient>();

            client.Post(new Request())
                .ReturnsForAnyArgs(
                    new UniTask<Response>(new Response(HttpStatusCode.OK, "{'token':'IDENTITY someToken'}")));

            IdentityService identityService = new IdentityService(server, client);
            Identity identity = await identityService.LogIn(this.identity, password, lifespan);

            Assert.IsNotNull(identity.Token);
            Assert.IsNotEmpty(identity.Token);
        });
    }
}