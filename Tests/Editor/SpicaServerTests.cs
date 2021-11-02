using System.Collections;
using System.Net;
using Cysharp.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SpicaSDK.Interfaces;
using SpicaSDK.Services;
using UnityEngine.TestTools;

namespace SpicaSDK.Tests.Editor
{
    public class SpicaServerTests
    {
        private string rootUrl = "testUrl";

        [UnityTest]
        public IEnumerator Connects() => UniTask.ToCoroutine(async delegate()
        {
            IHttpClient client = Substitute.For<IHttpClient>();
            client.Get(new Request())
                .ReturnsForAnyArgs(new UniTask<Response>(new Response(HttpStatusCode.OK, string.Empty)));

            ISpicaServer server = new SpicaServer(rootUrl, client);
            await server.Initialize();

            Assert.IsTrue(server.IsAvailable);
        });

        [UnityTest]
        public IEnumerator CantConnect() => UniTask.ToCoroutine(async delegate
        {
            IHttpClient client = Substitute.For<IHttpClient>();
            client.Get(new Request())
                .ReturnsForAnyArgs(new UniTask<Response>(new Response(HttpStatusCode.NotFound, string.Empty)));

            ISpicaServer server = new SpicaServer("falseUrl", client);
            await server.Initialize();

            Assert.IsFalse(server.IsAvailable);
        });
    }
}