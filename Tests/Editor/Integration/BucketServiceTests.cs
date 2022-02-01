using System;
using System.Collections;
using System.IO;
using System.Net;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using SpicaSDK.Interfaces;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using SpicaSDK.Services;
using SpicaSDK.Services.Models;
using SpicaSDK.Services.Services.Identity;
using UnityEngine;
using UnityEngine.TestTools;

namespace SpicaSDK.Tests.Editor.Integration
{
    public partial class BucketServiceTests
    {
        private class TestBucketDataModel
        {
            public readonly Id Id;

            [JsonProperty("title")] public string Title;
            [JsonProperty("description")] public string Description;

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

        [UnityTest]
        public IEnumerator GetThrowsUnauthorized() => UniTask.ToCoroutine(async delegate
        {
            IHttpClient httpClient = new HttpClient();
            ISpicaServer spicaServer = new SpicaServer(new SpicaServerUrl(url), httpClient);

            try
            {
                await new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>()).GetAsync(
                    new Id("testBucket"));
            }
            catch (UnityWebRequestException e)
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, (HttpStatusCode)e.ResponseCode);
            }
        });

        [UnityTest]
        public IEnumerator GetAll() => UniTask.ToCoroutine(async delegate
        {
            IHttpClient httpClient = new HttpClient();
            ISpicaServer spicaServer = new SpicaServer(new SpicaServerUrl(url), httpClient);

            IdentityService identityService = new IdentityService(spicaServer, httpClient);
            spicaServer.Identity = await identityService.LogInAsync("spica", "spica", float.MaxValue);

            Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
            var bucketService = new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>());
            await bucketService.CreateAsync(bucket);
            await bucketService.CreateAsync(bucket);

            var buckets = await bucketService.GetAllAsync();

            Assert.IsNotNull(buckets);
            Assert.IsNotNull(buckets.Length > 0);

            foreach (var bucket1 in buckets)
            {
                await bucketService.DeleteAsync(new Id(bucket1.Id));
            }
        });

        [UnityTest]
        public IEnumerator Get() => UniTask.ToCoroutine(async delegate
        {
            IHttpClient httpClient = new HttpClient();
            ISpicaServer spicaServer = new SpicaServer(new SpicaServerUrl(url), httpClient);

            IdentityService identityService = new IdentityService(spicaServer, httpClient);
            spicaServer.Identity = await identityService.LogInAsync("spica", "spica", float.MaxValue);

            Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
            var bucketService = new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>());
            Bucket newBucket = await bucketService.CreateAsync(bucket);

            await bucketService.GetAsync(new Id(newBucket.Id));

            Assert.Pass("Bucket fetch successfully");

            await bucketService.DeleteAsync(new Id(newBucket.Id));
        });

        private static string TestBucketDataAsJson =>
            File.ReadAllText($"{Application.dataPath}/SpicaSDK/Tests/Editor/TestAssets/Bucket.txt");

        [UnityTest]
        public IEnumerator Create() => UniTask.ToCoroutine(async delegate
        {
            IHttpClient httpClient = new HttpClient();
            ISpicaServer spicaServer = new SpicaServer(new SpicaServerUrl(url), httpClient);

            IdentityService identityService = new IdentityService(spicaServer, httpClient);
            spicaServer.Identity = await identityService.LogInAsync("spica", "spica", float.MaxValue);

            Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
            var bucketService = new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>());
            Bucket newBucket = await bucketService.CreateAsync(bucket);

            Assert.IsNotNull(newBucket);

            await bucketService.DeleteAsync(new Id(newBucket.Id));
        });

        [UnityTest]
        public IEnumerator Delete() => UniTask.ToCoroutine(async delegate
        {
            IHttpClient httpClient = new HttpClient();
            ISpicaServer spicaServer = new SpicaServer(new SpicaServerUrl(url), httpClient);

            IdentityService identityService = new IdentityService(spicaServer, httpClient);
            spicaServer.Identity = await identityService.LogInAsync("spica", "spica", float.MaxValue);

            Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
            Bucket newBucket =
                await new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>())
                    .CreateAsync(bucket);

            var status = await new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>())
                .DeleteAsync(new Id(newBucket.Id));

            Assert.AreEqual(HttpStatusCode.NoContent, status);
            Assert.Pass("Bucket fetch successfully");
        });
    }
}