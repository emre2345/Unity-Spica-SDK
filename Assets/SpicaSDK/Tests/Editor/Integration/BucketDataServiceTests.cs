using System.Collections;
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
        public class DataTests
        {
            async UniTask<(ISpicaServer spicaServer, IHttpClient httpClient)> Setup()
            {
                IHttpClient httpClient = new HttpClient();
                ISpicaServer spicaServer = new SpicaServer(new SpicaServerUrl(url), httpClient);

                IdentityService identityService = new IdentityService(spicaServer, httpClient);
                spicaServer.Identity = await identityService.LogInAsync("spica", "spica", float.MaxValue);

                return (spicaServer, httpClient);
            }

            [UnityTest]
            public IEnumerator Get() => UniTask.ToCoroutine(async delegate
            {
                (ISpicaServer spicaServer, IHttpClient httpClient) = await Setup();

                BucketService bucketService =
                    new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>());

                Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
                Bucket newBucket = await bucketService.CreateAsync(bucket);
                var newData =
                    await bucketService.Data.InsertAsync(new Id(newBucket.Id), new TestBucketDataModel("test", "test"));

                var data = await bucketService.Data.GetAsync<TestBucketDataModel>(new Id(newBucket.Id),
                    newData.Id,
                    new QueryParams());

                Assert.NotNull(data);
                Assert.AreEqual(newData.Id.Value, data.Id.Value);

                await bucketService.DeleteAsync(new Id(newBucket.Id));
            });

            [UnityTest]
            public IEnumerator GetAll() => UniTask.ToCoroutine(async delegate()
            {
                (ISpicaServer spicaServer, IHttpClient httpClient) = await Setup();

                BucketService bucketService =
                    new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>());

                Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
                Bucket newBucket = await bucketService.CreateAsync(bucket);
                await bucketService.Data.InsertAsync(new Id(newBucket.Id), new TestBucketDataModel("test", "test"));
                await bucketService.Data.InsertAsync(new Id(newBucket.Id), new TestBucketDataModel("test", "test"));

                var data = await bucketService.Data.GetAllAsync<TestBucketDataModel>(new Id(newBucket.Id),
                    new QueryParams());

                Assert.NotNull(data);
                Assert.IsTrue(data.Length > 0);

                await bucketService.DeleteAsync(new Id(newBucket.Id));
            });

            [UnityTest]
            public IEnumerator Insert() => UniTask.ToCoroutine(async delegate()
            {
                (ISpicaServer spicaServer, IHttpClient httpClient) = await Setup();

                var newData = new TestBucketDataModel("newData", "newData");

                BucketService bucketService =
                    new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>());

                Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
                Bucket newBucket = await bucketService.CreateAsync(bucket);

                var data = await bucketService.Data.InsertAsync<TestBucketDataModel>(new Id(newBucket.Id), newData);

                Assert.NotNull(data);
                Assert.NotNull(data.Id);
                Assert.IsTrue(!string.IsNullOrEmpty(data.Id.Value));
                await bucketService.DeleteAsync(new Id(newBucket.Id));
            });

            [UnityTest]
            public IEnumerator Patch() => UniTask.ToCoroutine(async delegate
            {
                (ISpicaServer spicaServer, IHttpClient httpClient) = await Setup();

                var newData = new TestBucketDataModel("newData", "newData");

                BucketService bucketService =
                    new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>());

                Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
                Bucket newBucket = await bucketService.CreateAsync(bucket);

                var insertedData =
                    await bucketService.Data.InsertAsync<TestBucketDataModel>(new Id(newBucket.Id), newData);
                var patchedData = await bucketService.Data.PatchAsync(new Id(newBucket.Id), insertedData.Id,
                    new TestBucketDataModel("patchedData", "patchedDesc"));

                Assert.NotNull(patchedData);
                await bucketService.DeleteAsync(new Id(newBucket.Id));
                // Assert.AreSame(insertedData.Id, patchedData.Id);
            });

            [UnityTest]
            public IEnumerator Remove() => UniTask.ToCoroutine(async delegate()
            {
                (ISpicaServer spicaServer, IHttpClient httpClient) = await Setup();

                var newData = new TestBucketDataModel("newData", "newData");

                BucketService bucketService =
                    new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>());

                Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
                Bucket newBucket = await bucketService.CreateAsync(bucket);

                var insertedData =
                    await bucketService.Data.InsertAsync<TestBucketDataModel>(new Id(newBucket.Id), newData);
                var deleted = await bucketService.Data.RemoveAsync(new Id(newBucket.Id), insertedData.Id);

                Assert.IsTrue(deleted);
                await bucketService.DeleteAsync(new Id(newBucket.Id));
            });

            [UnityTest]
            public IEnumerator Replace() => UniTask.ToCoroutine(async delegate()
            {
                (ISpicaServer spicaServer, IHttpClient httpClient) = await Setup();

                var newData = new TestBucketDataModel("newData", "newData");

                BucketService bucketService =
                    new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>());

                Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
                Bucket newBucket = await bucketService.CreateAsync(bucket);

                var insertedData =
                    await bucketService.Data.InsertAsync<TestBucketDataModel>(new Id(newBucket.Id), newData);
                var deleted = await bucketService.Data.ReplaceAsync(new Id(newBucket.Id), insertedData.Id,
                    new TestBucketDataModel("replacedData", "replacedData"));

                Assert.NotNull(deleted);
                Assert.AreEqual(insertedData.Id, deleted.Id);
                await bucketService.DeleteAsync(new Id(newBucket.Id));
            });
        }
    }
}