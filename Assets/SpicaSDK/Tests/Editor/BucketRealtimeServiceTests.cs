using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using SpicaSDK.Interfaces;
using SpicaSDK.Services;
using SpicaSDK.Services.Models;
using SpicaSDK.Services.WebSocketClient;
using UniRx;
using UnityEngine;
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
                IWebSocketConnection webSocketConnection = MockWebSocketConnection;

                var datas = new List<TestBucketDataModel>(TestDatas);
                var newData = new TestBucketDataModel("new", "new");

                // ---

                webSocketClient.Connect(string.Empty).ReturnsForAnyArgs(webSocketConnection);
                webSocketConnection.Subscribe(Arg.Any<IObserver<Message>>()).Returns(delegate(CallInfo info)
                {
                    return datas.ObserveEveryValueChanged(list => list.Count).Select(list =>
                            new Message(DataChangeType.Insert, HttpStatusCode.OK, JsonConvert.SerializeObject(newData)))
                        .Subscribe(info.Arg<IObserver<Message>>());
                });

                // ---

                BucketService bucketService = new BucketService(server, httpClient, webSocketClient);
                BucketConnection<TestBucketDataModel> bucketConnection =
                    bucketService.Realtime.ConnectToBucket<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());

                // ---

                CancellationTokenSource source = new CancellationTokenSource();
                SkipInitialData(bucketConnection).Subscribe(message =>
                {
                    Assert.IsTrue(message.ChangeType == DataChangeType.Insert);
                    source.Cancel();
                });

                // ---

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(
                    unit => datas.Add(newData)
                );

                return UniTask.WaitUntilCanceled(source.Token).Timeout(TimeSpan.FromSeconds(1));
            });

            [UnityTest]
            public IEnumerator InsertToBucket() => UniTask.ToCoroutine(delegate()
            {
                CancellationTokenSource source = new CancellationTokenSource();

                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient httpClient = Substitute.For<IHttpClient>();
                IWebSocketClient webSocketClient = MockWebSocketClient;
                IWebSocketConnection webSocketConnection = MockWebSocketConnection;

                var datas = new List<TestBucketDataModel>(TestDatas);
                var newData = new TestBucketDataModel("new", "new");

                // ---

                webSocketClient.Connect(Arg.Any<string>()).Returns(webSocketConnection);
                webSocketConnection.When(connection => connection.SendMessage(Arg.Any<string>())).Do(
                    delegate(CallInfo info)
                    {
                        var arg = info.Arg<string>();
                        var message = new { @Event = string.Empty, data = new TestBucketDataModel() };
                        var serverMessage = JsonConvert.DeserializeAnonymousType(arg, message);
                        Debug.Log($"Adding data: {arg}");
                        datas.Add(serverMessage.data);
                    });

                webSocketConnection.Subscribe(Arg.Any<IObserver<Message>>()).Returns(delegate(CallInfo info)
                {
                    return datas.ObserveEveryValueChanged(list => list.Count).Skip(1).Where(i => i > TestDatas.Length).Select(
                            list =>
                                new Message(DataChangeType.Insert, HttpStatusCode.OK,
                                    JsonConvert.SerializeObject(newData)))
                        .Subscribe(info.Arg<IObserver<Message>>());
                });
                // ---

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

                return UniTask.WaitUntilCanceled(source.Token).Timeout(TimeSpan.FromSeconds(1));
            });

            [UnityTest]
            public IEnumerator DeleteFromBucket() => UniTask.ToCoroutine(delegate()
            {
                CancellationTokenSource source = new CancellationTokenSource();

                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient httpClient = Substitute.For<IHttpClient>();
                IWebSocketClient webSocketClient = MockWebSocketClient;

                var datas = new List<TestBucketDataModel>(TestDatas);
                var deletedData = datas[0];

                webSocketClient.Connect(String.Empty).ReturnsForAnyArgs(delegate(CallInfo info)
                {
                    return datas.ObserveEveryValueChanged(list => list.Count).Select(i =>
                    {
                        return new Message(DataChangeType.Delete, HttpStatusCode.OK,
                            JsonConvert.SerializeObject(deletedData));
                    });
                });
                // webSocketClient.When(client => client.SendMessage(Arg.Any<string>()))
                // .Do(info => datas.Remove(deletedData));

                BucketService bucketService = new BucketService(server, httpClient, webSocketClient);
                BucketConnection<TestBucketDataModel> bucketConnection =
                    bucketService.Realtime.ConnectToBucket<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());

                SkipInitialData(bucketConnection).Subscribe(change =>
                {
                    Assert.IsTrue(change.ChangeType == DataChangeType.Delete);
                    Assert.IsTrue(change.Document.Title == deletedData.Title &&
                                  change.Document.Description == deletedData.Description);
                    source.Cancel();
                });

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(unit => bucketConnection.Delete(deletedData));

                return UniTask.WaitUntilCanceled(source.Token);
            });

            [UnityTest]
            public IEnumerator PatchBucket() => UniTask.ToCoroutine(delegate()
            {
                CancellationTokenSource source = new CancellationTokenSource();

                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient httpClient = Substitute.For<IHttpClient>();
                IWebSocketClient webSocketClient = MockWebSocketClient;

                var datas = new List<TestBucketDataModel>(TestDatas);
                var patchedData = datas[0];

                webSocketClient.Connect(String.Empty).ReturnsForAnyArgs(delegate(CallInfo info)
                {
                    return datas.ObserveEveryValueChanged(list => list.Count).Select(i =>
                    {
                        return new Message(DataChangeType.Delete, HttpStatusCode.OK,
                            JsonConvert.SerializeObject(patchedData));
                    });
                });
                // webSocketClient.When(client => client.SendMessage(Arg.Any<string>()))
                // .Do(info => datas.Remove(patchedData));

                BucketService bucketService = new BucketService(server, httpClient, webSocketClient);
                BucketConnection<TestBucketDataModel> bucketConnection =
                    bucketService.Realtime.ConnectToBucket<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());

                SkipInitialData(bucketConnection).Subscribe(change =>
                {
                    Assert.IsTrue(change.ChangeType == DataChangeType.Delete);
                    Assert.IsTrue(change.Document.Title == patchedData.Title &&
                                  change.Document.Description == patchedData.Description);
                    source.Cancel();
                });

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(unit => bucketConnection.Patch(patchedData));

                return UniTask.WaitUntilCanceled(source.Token);
            });

            [UnityTest]
            public IEnumerator CloseConnectionWhenStreamDisposed() => UniTask.ToCoroutine(delegate()
            {
                CancellationTokenSource source = new CancellationTokenSource();

                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient httpClient = Substitute.For<IHttpClient>();
                IWebSocketClient webSocketClient = MockWebSocketClient;

                var socket = Substitute.For<WebSocket>();
                var connection = new WebSocketConnection(socket);

                webSocketClient.Connect(Arg.Any<string>()).Returns(info => connection);

                BucketService bucketService = new BucketService(server, httpClient, webSocketClient);
                BucketConnection<TestBucketDataModel> bucketConnection =
                    bucketService.Realtime.ConnectToBucket<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());


                bucketConnection.Dispose();
                // webSocketClient.Received().Disconnect();

                return UniTask.WaitUntilCanceled(source.Token);
            });
        }
    }
}