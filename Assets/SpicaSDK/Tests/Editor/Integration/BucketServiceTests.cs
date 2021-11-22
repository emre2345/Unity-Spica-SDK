using System;
using System.Collections;
using System.Net;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using SpicaSDK.Interfaces;
using SpicaSDK.Services;
using SpicaSDK.Services.Models;
using SpicaSDK.Services.Services.Identity;
using UnityEngine.TestTools;

namespace SpicaSDK.Tests.Editor.Integration
{
    public partial class BucketServiceTests
    {
        private class TestBucketDataModel
        {
            public readonly Id Id;
            
            [JsonProperty("title")]
            public string Title;
            [JsonProperty("description")]
            public string Description;

            public TestBucketDataModel()
            {
            }

            public TestBucketDataModel(string title, string description)
            {
                Title = title;
                Description = description;
            }

            [JsonConstructor]
            public TestBucketDataModel(string _id, string title, string description)
            {
                Id = new Id(_id);
                Title = title;
                Description = description;
            }

            public override string ToString()
            {
                return $"{{\"_id\": \"{Id}\", \"title\": \"{Title}\", \"description\": \"{Description}\"}}";
            }
        }

        private static string url = "http://localhost:4500";
        private static string testBucketId = "619b89fd9e6618002e30414e";

        [UnityTest]
        public IEnumerator GetThrowsUnauthorized() => UniTask.ToCoroutine(async delegate
        {
            IHttpClient httpClient = new HttpClient();
            ISpicaServer spicaServer = new SpicaServer(url, httpClient);

            try
            {
                await new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>()).Get(
                    new Id("testBucket"));
            }
            catch (UnityWebRequestException e)
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, (HttpStatusCode)e.ResponseCode);
            }
        });

        [UnityTest]
        public IEnumerator Get() => UniTask.ToCoroutine(async delegate
        {
            IHttpClient httpClient = new HttpClient();
            ISpicaServer spicaServer = new SpicaServer(url, httpClient);

            IdentityService identityService = new IdentityService(spicaServer, httpClient);
            spicaServer.Identity = await identityService.LogIn("spica", "spica", float.MaxValue);

            await new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>()).Get(
                new Id(testBucketId));

            Assert.Pass("Bucket fetch successfully");
        });
    }
}