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
    }
}