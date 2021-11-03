using System.Collections;
using System.Collections.Generic;
using System.Net;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using SpicaSDK.Interfaces;
using SpicaSDK.Services;
using SpicaSDK.Services.Models;
using UnityEngine.TestTools;

namespace SpicaSDK.Tests.Editor
{
    public partial class BucketServiceTests
    {
        public class DataTests
        {
            private class TestBucketDataModel
            {
                public readonly Id Id;
                public readonly string Title;
                public readonly string Description;

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

            [UnityTest]
            public IEnumerator Insert() => UniTask.ToCoroutine(async () =>
            {
                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient client = Substitute.For<IHttpClient>();

                var newData = new TestBucketDataModel("testTitle1", "testDesc");

                List<TestBucketDataModel> testData = new List<TestBucketDataModel>(TestDatas);
                client.Post(new Request(server.BucketDataUrl(new Id(TestBucketId)),
                    JsonConvert.SerializeObject(newData))).Returns(delegate(CallInfo info)
                {
                    testData.Add(newData);
                    return new UniTask<Response>(new Response(HttpStatusCode.OK,
                        JsonConvert.SerializeObject(new TestBucketDataModel("wejfoiwejf", newData.Title,
                            newData.Description))));
                });

                BucketService bucketService = new BucketService(server, client);
                TestBucketDataModel data =
                    await bucketService.Data.Insert<TestBucketDataModel>(new Id(TestBucketId), newData);

                Assert.IsNotNull(data);
                Assert.IsTrue(testData.Count == 4);
            });

            [UnityTest]
            public IEnumerator Patch() => UniTask.ToCoroutine(async delegate
            {
                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient client = Substitute.For<IHttpClient>();

                var testData = TestDatas[0];
                var patchedData = new TestBucketDataModel("patchedTitle", "patchedDesc");

                BucketService bucketService = new BucketService(server, client);
                var data = await bucketService.Data.Patch(new Id(TestBucketId), testData.Id, patchedData);

                Assert.IsNotNull(data);
            });

            [UnityTest]
            public IEnumerator Remove() => UniTask.ToCoroutine(async delegate
            {
                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient client = Substitute.For<IHttpClient>();

                var testData = TestDatas[0];

                BucketService bucketService = new BucketService(server, client);
                var data = await bucketService.Data.Remove(new Id(TestBucketId), testData.Id);

                Assert.IsNotNull(data);
            });
        }
    }
}