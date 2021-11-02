using System;
using System.Collections;
using System.Collections.Generic;
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
    public class BucketServiceTests
    {
        private static string TestBucketDataAsJson =>
            File.ReadAllText($"{Application.dataPath}/SpicaSDK/Tests/Editor/TestAssets/Bucket.txt");

        private static string TestBucketId =>
            JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson).Id;

        private static string TestBucketData =>
            File.ReadAllText($"{Application.dataPath}/SpicaSDK/Tests/Editor/TestAssets/BucketData.txt");

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
                await new BucketService(server, client).Get(new Id(bucketId));
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

            BucketService bucketService = new BucketService(server, client);
            Bucket bucket = await bucketService.Get(new Id(bucketId));

            Assert.NotNull(bucket);
            Assert.IsTrue(bucketId.Equals(bucket.Id));
        });

        public class DataTests
        {
            private class TestBucketDataModel
            {
                public readonly Id Id;
                public readonly string Title;
                public readonly string Description;

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

            private TestBucketDataModel[] TestDatas =>
                JsonConvert.DeserializeObject<TestBucketDataModel[]>(TestBucketData);

            [UnityTest]
            public IEnumerator Get() => UniTask.ToCoroutine(async delegate
            {
                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient client = Substitute.For<IHttpClient>();

                var firstData = TestDatas[0];

                client.Get(new Request(server.BucketDataDocumentUrl(new Id(TestBucketId), firstData.Id)))
                    .Returns(info => new UniTask<Response>(
                        new Response(HttpStatusCode.OK, firstData.ToString())));

                BucketService bucketService = new BucketService(server, client);
                var data = await bucketService.Data.Get<TestBucketDataModel>(new Id(TestBucketId), firstData.Id,
                    new QueryParams());

                Assert.IsTrue(data.Id.Equals(firstData.Id));
            });

            [UnityTest]
            public IEnumerator GetAll() => UniTask.ToCoroutine(async () =>
            {
                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient client = Substitute.For<IHttpClient>();

                client.Get(new Request(server.BucketDataUrl(new Id(TestBucketId)))).Returns(info =>
                    new UniTask<Response>(new Response(HttpStatusCode.OK, TestBucketData)));

                BucketService bucketService = new BucketService(server, client);
                var data = await bucketService.Data.GetAll<TestBucketDataModel>(new Id(TestBucketId),
                    new QueryParams());

                Assert.IsTrue(data.Length == 3);
            });
        }
    }
}