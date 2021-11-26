using System;
using System.Collections;
using System.Net;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using SpicaSDK.Interfaces;
using SpicaSDK.Services;
using UnityEngine.TestTools;

namespace SpicaSDK.Tests.Editor.Integration
{
    public class SpicaServerTests
    {
        private string url = "http://localhost:4500/passport/identify";

        [UnityTest]
        public IEnumerator Connects() => UniTask.ToCoroutine(async delegate
        {
            IHttpClient client = new HttpClient();
            ISpicaServer server = new SpicaServer(url, client);
            await server.InitializeAsync();

            Assert.IsTrue(server.IsAvailable);
        });

        [UnityTest]
        public IEnumerator CantConnects() => UniTask.ToCoroutine(async delegate
        {
            IHttpClient client = new HttpClient();
            ISpicaServer server = new SpicaServer("falseUrl", client);

            try
            {
                await server.InitializeAsync();
            }
            catch (UnityWebRequestException e)
            {
                Assert.Pass($"Exception thrown: {e.Message}");
            }
        });
    }
}