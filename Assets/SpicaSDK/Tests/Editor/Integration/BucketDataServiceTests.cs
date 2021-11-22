using System.Collections;
using Cysharp.Threading.Tasks;
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
            private string testDataId = "619b8fb99e6618002e304164";

            async UniTask<(ISpicaServer spicaServer, IHttpClient httpClient)> Setup()
            {
                IHttpClient httpClient = new HttpClient();
                ISpicaServer spicaServer = new SpicaServer(url, httpClient);

                IdentityService identityService = new IdentityService(spicaServer, httpClient);
                spicaServer.Identity = await identityService.LogIn("spica", "spica", float.MaxValue);

                return (spicaServer, httpClient);
            }

            [UnityTest]
            public IEnumerator Get() => UniTask.ToCoroutine(async delegate
            {
                (ISpicaServer spicaServer, IHttpClient httpClient) = await Setup();

                BucketService bucketService =
                    new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>());
                var data = await bucketService.Data.Get<TestBucketDataModel>(new Id(testBucketId), new Id(testDataId),
                    new QueryParams());

                Assert.NotNull(data);
                Assert.AreEqual(testDataId, data.Id.Value);
            });

            [UnityTest]
            public IEnumerator GetAll() => UniTask.ToCoroutine(async delegate()
            {
                (ISpicaServer spicaServer, IHttpClient httpClient) = await Setup();

                BucketService bucketService =
                    new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>());
                var data = await bucketService.Data.GetAll<TestBucketDataModel>(new Id(testBucketId),
                    new QueryParams());
                
                Assert.NotNull(data);
                Assert.IsTrue(data.Length > 0);
            });
            
            [UnityTest]
            public IEnumerator Insert() => UniTask.ToCoroutine(async delegate()
            {
                (ISpicaServer spicaServer, IHttpClient httpClient) = await Setup();

                var newData = new TestBucketDataModel("newData", "newData");

                BucketService bucketService =
                    new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>());
                var data = await bucketService.Data.Insert<TestBucketDataModel>(new Id(testBucketId), newData);
                
                Assert.NotNull(data);
                Assert.NotNull(data.Id);
                Assert.IsTrue(!string.IsNullOrEmpty(data.Id.Value));
            });
            
            [UnityTest]
            public IEnumerator Patch() => UniTask.ToCoroutine(async delegate
            {
                (ISpicaServer spicaServer, IHttpClient httpClient) = await Setup();

                var newData = new TestBucketDataModel("newData", "newData");

                BucketService bucketService =
                    new BucketService(spicaServer, httpClient, Substitute.For<IWebSocketClient>());
                var insertedData = await bucketService.Data.Insert<TestBucketDataModel>(new Id(testBucketId), newData);
                var patchedData = await bucketService.Data.Patch(new Id(testBucketId), insertedData.Id,
                    new TestBucketDataModel("patchedData", "patchedDesc"));

                Assert.NotNull(patchedData);
                Assert.AreSame(patchedData.Id, insertedData.Id);
            });
        }
    }
}