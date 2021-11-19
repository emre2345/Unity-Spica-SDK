using System;
using System.Collections;
using System.IO;
using System.Net;
using Cysharp.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SpicaSDK.Interfaces;
using SpicaSDK.Services;
using SpicaSDK.Services.Models;
using SpicaSDK.Services.Services.Identity;
using SpicaSDK.Services.Services.Identity.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.TestTools;

namespace SpicaSDK.Tests.Editor
{
    public partial class BucketServiceTests
    {
        private class TestBucketDataModel
        {
            public readonly Id Id;
            public string Title;
            public string Description;

            public TestBucketDataModel()
            {
            }

            public TestBucketDataModel(string title, string description)
            {
                Title = title;
                Description = description;
            }

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

        private static TestBucketDataModel[] TestDatas =>
            JsonConvert.DeserializeObject<TestBucketDataModel[]>(TestBucketData);

        private static string TestBucketDataAsJson =>
            File.ReadAllText($"{Application.dataPath}/SpicaSDK/Tests/Editor/TestAssets/Bucket.txt");

        private static string TestBucketId =>
            JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson).Id;

        private static string TestBucketData =>
            File.ReadAllText($"{Application.dataPath}/SpicaSDK/Tests/Editor/TestAssets/BucketData.txt");

        private static IWebSocketClient MockWebSocketClient => Substitute.For<IWebSocketClient>();

        private static IWebSocketConnection MockWebSocketConnection => Substitute.For<IWebSocketConnection>();

        [UnityTest]
        public IEnumerator GetThrowsUnauthorized() => UniTask.ToCoroutine(async delegate
        {
            string bucketId = TestBucketId;

            ISpicaServer server = Substitute.For<ISpicaServer>();
            IHttpClient client = Substitute.For<IHttpClient>();

            client.Get(new Request(server.BucketUrl(new Id(bucketId)))).Returns(info =>
                new UniTask<Response>(new Response(HttpStatusCode.Unauthorized,
                    "{\"statusCode\":401,\"message\":\"No auth token\",\"error\":\"Unauthorized\"}")));

            try
            {
                await new BucketService(server, client, MockWebSocketClient).Get(new Id(bucketId));
            }
            catch (UnauthorizedAccessException e)
            {
                Assert.Pass();
            }
        });

        [UnityTest]
        public IEnumerator Get() => UniTask.ToCoroutine(async delegate
        {
            string bucketId = TestBucketId;

            ISpicaServer server = Substitute.For<ISpicaServer>();
            IHttpClient client = Substitute.For<IHttpClient>();

            client.Post(new Request())
                .ReturnsForAnyArgs(
                    new UniTask<Response>(new Response(HttpStatusCode.OK, "{'token':'IDENTITY someToken'}")));

            client.Get(new Request(server.BucketUrl(new Id(bucketId)))).Returns(info =>
                new UniTask<Response>(new Response(HttpStatusCode.OK, TestBucketDataAsJson)));

            IdentityService identityService = new IdentityService(server, client);
            Identity identity = await identityService.LogIn("identity", "password", float.MaxValue);
            server.Identity = identity;

            BucketService bucketService = new BucketService(server, client, MockWebSocketClient);
            Bucket bucket = await bucketService.Get(new Id(bucketId));

            Assert.NotNull(bucket);
            Assert.IsTrue(bucketId.Equals(bucket.Id));
        });
    }
}