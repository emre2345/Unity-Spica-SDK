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
            [UnityTest]
            public IEnumerator Get() => UniTask.ToCoroutine(async delegate
            {
                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient client = Substitute.For<IHttpClient>();

                var firstData = TestDatas[0];

                client.Get(new Request(server.BucketDataDocumentUrl(new Id(TestBucketId), firstData.Id)))
                    .Returns(info => new UniTask<Response>(
                        new Response(HttpStatusCode.OK, firstData.ToString())));

                BucketService bucketService = new BucketService(server, client, MockWebSocketClient);
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

                BucketService bucketService = new BucketService(server, client, MockWebSocketClient);
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

                BucketService bucketService = new BucketService(server, client, MockWebSocketClient);
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

                client.Patch(new Request(server.BucketDataUrl(new Id(TestBucketId)),
                    JsonConvert.SerializeObject(patchedData))).Returns(
                    delegate(CallInfo info)
                    {
                        testData.Title = patchedData.Title;
                        testData.Description = patchedData.Description;
                        return new UniTask<Response>(new Response(HttpStatusCode.OK,
                            JsonConvert.SerializeObject(testData)));
                    });

                BucketService bucketService = new BucketService(server, client, MockWebSocketClient);
                var data = await bucketService.Data.Patch(new Id(TestBucketId), testData.Id, patchedData);

                Assert.IsNotNull(data);
            });

            [UnityTest]
            public IEnumerator Remove() => UniTask.ToCoroutine(async delegate
            {
                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient client = Substitute.For<IHttpClient>();

                var datas = new List<TestBucketDataModel>(TestDatas);
                var testData = TestDatas[0];

                client.Delete(new Request()).ReturnsForAnyArgs(
                    delegate(CallInfo info)
                    {
                        datas.RemoveAt(0);
                        return new UniTask<Response>(new Response(HttpStatusCode.OK,
                            JsonConvert.SerializeObject(testData)));
                    });

                BucketService bucketService = new BucketService(server, client, MockWebSocketClient);
                await bucketService.Data.Remove(new Id(TestBucketId), testData.Id);

                Assert.IsNotNull(datas.Count < TestDatas.Length);
            });

            [UnityTest]
            public IEnumerator Replace() => UniTask.ToCoroutine(async delegate
            {
                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient client = Substitute.For<IHttpClient>();

                var datas = new List<TestBucketDataModel>(TestDatas);
                var replacedData = new TestBucketDataModel("replacedTitle", "replacedDesc");

                client.Put(new Request()).ReturnsForAnyArgs(
                    delegate(CallInfo info)
                    {
                        datas[0] = replacedData;
                        return new UniTask<Response>(new Response(HttpStatusCode.OK,
                            JsonConvert.SerializeObject(replacedData)));
                    });

                BucketService bucketService = new BucketService(server, client, MockWebSocketClient);
                await bucketService.Data.Replace<TestBucketDataModel>(new Id(TestBucketId), datas[0].Id,
                    replacedData);

                Assert.IsNotNull(datas[0].Title == replacedData.Title &&
                                 datas[0].Description == replacedData.Description);
            });
        }
    }
}