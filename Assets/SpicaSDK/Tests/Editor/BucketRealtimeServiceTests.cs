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
                    return firstData.ObserveEveryValueChanged(model => model.Title).Skip(1).Select(s =>
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
                documentConnection.Subscribe(message =>
                {
                    Assert.IsTrue(message.Title.Equals(newTitle));
                    source.Cancel();
                });

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(unit => firstData.Title = newTitle);

                return UniTask.WaitUntilCanceled(source.Token);
            });


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
                    return datas.ObserveEveryValueChanged(list => list.Count).Skip(1).Select(list =>
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
                bucketConnection.Subscribe(message =>
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

            private void WhenSentMessageThroughWebSocketDo(IWebSocketConnection connection,
                Action<Tuple<string, TestBucketDataModel>> doDlg) =>
                connection.When(socketConnection => socketConnection.SendMessage(Arg.Any<string>())).Do(
                    delegate(CallInfo info)
                    {
                        var arg = info.Arg<string>();
                        var message = new {@Event = string.Empty, data = new TestBucketDataModel()};
                        var serverMessage = JsonConvert.DeserializeAnonymousType(arg, message);
                        Debug.Log($"Server Message: {arg}");
                        doDlg(new Tuple<string, TestBucketDataModel>(serverMessage.Event, serverMessage.data));
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
                WhenSentMessageThroughWebSocketDo(webSocketConnection, message => datas.Add(message.Item2));

                webSocketConnection.Subscribe(Arg.Any<IObserver<Message>>()).Returns(delegate(CallInfo info)
                {
                    return datas.ObserveEveryValueChanged(list => list.Count).Skip(1).Where(i => i > TestDatas.Length)
                        .Select(
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

                bucketConnection.Subscribe(message =>
                {
                    Assert.IsTrue(message.ChangeType == DataChangeType.Insert);
                    Assert.IsTrue(message.Document.Title == newData.Title &&
                                  message.Document.Description == newData.Description);
                    source.Cancel();
                });

                Observable.ReturnUnit().DelayFrame(5).Subscribe(unit => bucketConnection.Insert(newData));

                return UniTask.WaitUntilCanceled(source.Token).Timeout(TimeSpan.FromSeconds(1));
            });

            [UnityTest]
            public IEnumerator DeleteFromBucket() => UniTask.ToCoroutine(delegate()
            {
                CancellationTokenSource source = new CancellationTokenSource();

                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient httpClient = Substitute.For<IHttpClient>();
                IWebSocketClient webSocketClient = MockWebSocketClient;
                IWebSocketConnection webSocketConnection = MockWebSocketConnection;

                var datas = new List<TestBucketDataModel>(TestDatas);
                var deletedData = datas[0];

                webSocketClient.Connect(String.Empty).ReturnsForAnyArgs(webSocketConnection);
                WhenSentMessageThroughWebSocketDo(webSocketConnection, message => datas.Remove(deletedData));

                webSocketConnection.Subscribe(Arg.Any<IObserver<Message>>()).ReturnsForAnyArgs(delegate(CallInfo info)
                {
                    return datas.ObserveEveryValueChanged(list => list.Count).Skip(1).Where(i => i < TestDatas.Length)
                        .Select(i =>
                        {
                            return new Message(DataChangeType.Delete, HttpStatusCode.OK,
                                JsonConvert.SerializeObject(deletedData));
                        }).Subscribe(info.Arg<IObserver<Message>>());
                });

                BucketService bucketService = new BucketService(server, httpClient, webSocketClient);
                BucketConnection<TestBucketDataModel> bucketConnection =
                    bucketService.Realtime.ConnectToBucket<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());

                bucketConnection.Subscribe(change =>
                {
                    Assert.IsTrue(change.ChangeType == DataChangeType.Delete);
                    Assert.IsTrue(change.Document.Title == deletedData.Title &&
                                  change.Document.Description == deletedData.Description);
                    source.Cancel();
                });

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(unit => bucketConnection.Delete(deletedData));

                return UniTask.WaitUntilCanceled(source.Token).Timeout(TimeSpan.FromSeconds(1));
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

                bucketConnection.Subscribe(change =>
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
                IWebSocketConnection webSocketConnection = MockWebSocketConnection;

                webSocketClient.Connect(Arg.Any<string>()).Returns(info => webSocketConnection);

                BucketService bucketService = new BucketService(server, httpClient, webSocketClient);
                BucketConnection<TestBucketDataModel> bucketConnection =
                    bucketService.Realtime.ConnectToBucket<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());

                webSocketConnection.When(socketConnection => socketConnection.Disconnect()).Do(info =>
                {
                    source.Cancel();
                });
                bucketConnection.Dispose();

                return UniTask.WaitUntilCanceled(source.Token).Timeout(TimeSpan.FromSeconds(1));
            });
        }
    }
}