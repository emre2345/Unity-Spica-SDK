using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using SpicaSDK.Interfaces;
using SpicaSDK.Services;
using SpicaSDK.Services.Models;
using SpicaSDK.Services.WebSocketClient;
using UniRx;
using UnityEngine.TestTools;

namespace SpicaSDK.Tests.Editor
{
    public partial class BucketServiceTests
    {
        public class BucketRealtimeServiceTests
        {
            [UnityTest]
            public IEnumerator WatchDocument() => UniTask.ToCoroutine(delegate()
            {
                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient httpClient = Substitute.For<IHttpClient>();
                IWebSocketClient webSocketClient = MockWebSocketClient;

                var firstData = TestDatas[0];

                webSocketClient.Connect(string.Empty).ReturnsForAnyArgs(delegate(CallInfo info)
                {
                    return firstData.ObserveEveryValueChanged(model => model.Title).Select(s =>
                    {
                        var newData = new TestBucketDataModel(firstData.Id.Value, s, firstData.Description);
                        return new Message(DataChangeType.Update, HttpStatusCode.OK,
                            JsonConvert.SerializeObject(newData));
                    });
                });

                BucketService bucketService = new BucketService(server, httpClient, webSocketClient);
                DocumentChange<TestBucketDataModel> documentConnection =
                    bucketService.Realtime.WatchDocument<TestBucketDataModel>(new Id(TestBucketId), firstData.Id);

                string newTitle = "newTitle";

                CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource();
                SkipInitialData(documentConnection).Subscribe(message =>
                {
                    Assert.IsTrue(message.Title.Equals(newTitle));
                    source.Cancel();
                });

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(unit => firstData.Title = newTitle);

                return UniTask.WaitUntilCanceled(source.Token);
            });

            private IObservable<T> SkipInitialData<T>(IObservable<T> stream) =>
                stream.Skip(1);


            [UnityTest]
            public IEnumerator WatchBucket() => UniTask.ToCoroutine(delegate()
            {
                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient httpClient = Substitute.For<IHttpClient>();
                IWebSocketClient webSocketClient = MockWebSocketClient;

                var datas = new List<TestBucketDataModel>(TestDatas);
                var newData = new TestBucketDataModel("new", "new");

                webSocketClient.Connect(string.Empty).ReturnsForAnyArgs(delegate(CallInfo info)
                {
                    return datas.ObserveEveryValueChanged(list => list.Count).Select(list =>
                        new Message(DataChangeType.Insert, HttpStatusCode.OK, JsonConvert.SerializeObject(newData)));
                });

                BucketService bucketService = new BucketService(server, httpClient, webSocketClient);
                BucketConnection<TestBucketDataModel> bucketConnection =
                    bucketService.Realtime.ConnectToBucket<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());

                CancellationTokenSource source = new CancellationTokenSource();
                SkipInitialData(bucketConnection).Subscribe(message =>
                {
                    Assert.IsTrue(message.ChangeType == DataChangeType.Insert);
                    source.Cancel();
                });
                //
                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(
                    unit => datas.Add(newData)
                );

                return UniTask.WaitUntilCanceled(source.Token);
            });

            [UnityTest]
            public IEnumerator InsertToBucket() => UniTask.ToCoroutine(delegate()
            {
                CancellationTokenSource source = new CancellationTokenSource();

                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient httpClient = Substitute.For<IHttpClient>();
                IWebSocketClient webSocketClient = MockWebSocketClient;

                var datas = new List<TestBucketDataModel>(TestDatas);
                var newData = new TestBucketDataModel("new", "new");

                webSocketClient.Connect(string.Empty).ReturnsForAnyArgs(delegate(CallInfo info)
                {
                    return datas.ObserveEveryValueChanged(list => list.Count).Select(list =>
                        new Message(DataChangeType.Insert, HttpStatusCode.OK, JsonConvert.SerializeObject(newData)));
                });
                webSocketClient.When(client => client.SendMessage(Arg.Any<string>())).Do(info => datas.Add(newData));

                BucketService bucketService = new BucketService(server, httpClient, webSocketClient);
                BucketConnection<TestBucketDataModel> bucketConnection =
                    bucketService.Realtime.ConnectToBucket<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());


                SkipInitialData(bucketConnection).Subscribe(message =>
                {
                    Assert.IsTrue(message.ChangeType == DataChangeType.Insert);
                    Assert.IsTrue(message.Document.Title == newData.Title &&
                                  message.Document.Description == newData.Description);
                    source.Cancel();
                });

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(unit => bucketConnection.Insert(newData));


                return UniTask.WaitUntilCanceled(source.Token);
            });

            [UnityTest]
            public IEnumerator DeleteFromBucket() => UniTask.ToCoroutine(async delegate() { });

            [UnityTest]
            public IEnumerator PatchBucket() => UniTask.ToCoroutine(async delegate() { Assert.Fail(); });

            [UnityTest]
            public IEnumerator CloseConnectionWhenStreamDisposed() => UniTask.ToCoroutine(async delegate() { });
        }
    }
}