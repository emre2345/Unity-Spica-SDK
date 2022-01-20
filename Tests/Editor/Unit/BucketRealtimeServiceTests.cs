using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using SpicaSDK.Interfaces;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Services;
using SpicaSDK.Services.Models;
using SpicaSDK.Services.WebSocketClient;
using UniRx;
using UnityEngine.TestTools;

namespace SpicaSDK.Tests.Editor.Unit
{
    public partial class BucketServiceTests
    {
        public class BucketRealtimeServiceTests
        {
            private void WhenSentMessageThroughWebSocketDo(IBucketRealtimeConnection connection,
                Action<Tuple<string, TestBucketDataModel>> doDlg) =>
                connection.When(socketConnection => socketConnection.SendMessageAsync(Arg.Any<string>())).Do(
                    delegate(CallInfo info)
                    {
                        var arg = info.Arg<string>();
                        var message = new { @Event = string.Empty, data = new TestBucketDataModel() };
                        var serverMessage = JsonConvert.DeserializeAnonymousType(arg, message);
                        doDlg(new Tuple<string, TestBucketDataModel>(serverMessage.Event, serverMessage.data));
                    });

            private void WhenWebSocketSubscribed(IBucketRealtimeConnection connection,
                Func<CallInfo, IDisposable> dlg) =>
                connection.Subscribe(Arg.Any<IObserver<ServerMessage>>()).Returns(dlg);

            private (BucketService, IWebSocketClient) MockBucketService
            {
                get
                {
                    ISpicaServer server = Substitute.For<ISpicaServer>();
                    IHttpClient httpClient = Substitute.For<IHttpClient>();
                    IWebSocketClient webSocketClient = MockWebSocketClient;

                    var service = new BucketService(server, httpClient, webSocketClient);
                    return (service, webSocketClient);
                }
            }

            [UnityTest]
            public IEnumerator WatchDocument() => UniTask.ToCoroutine(async delegate()
            {
                (BucketService bucketService, IWebSocketClient webSocketClient) = MockBucketService;
                IBucketRealtimeConnection bucketRealtimeConnection = MockBucketRealtimeConnection;

                var firstData = TestDatas[0];

                webSocketClient.ConnectAsync(string.Empty, socket => new BucketRealtimeConnection(socket))
                    .ReturnsForAnyArgs(new UniTask<IWebSocketConnection>(bucketRealtimeConnection));
                WhenWebSocketSubscribed(bucketRealtimeConnection, delegate(CallInfo info)
                {
                    return firstData.ObserveEveryValueChanged(model => model.Title).Skip(1).Select(s =>
                    {
                        var newData = new TestBucketDataModel(firstData.Id.Value, s, firstData.Description);
                        return new ServerMessage(DataChangeType.Update,
                            JsonConvert.SerializeObject(newData));
                    }).Subscribe(info.Arg<IObserver<ServerMessage>>());
                });

                DocumentChange<TestBucketDataModel> documentConnection =
                    await bucketService.Realtime.WatchDocumentAsync<TestBucketDataModel>(new Id(TestBucketId),
                        firstData.Id);

                string newTitle = "newTitle";

                CancellationTokenSource source = new CancellationTokenSource();
                documentConnection.Subscribe(message =>
                {
                    Assert.IsTrue(message.Title.Equals(newTitle));
                    source.Cancel();
                });

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(unit => firstData.Title = newTitle);

                await UniTask.WaitUntilCanceled(source.Token).Timeout(TimeSpan.FromSeconds(1));
            });


            [UnityTest]
            public IEnumerator WatchBucket() => UniTask.ToCoroutine(async delegate()
            {
                (BucketService bucketService, IWebSocketClient webSocketClient) = MockBucketService;
                IBucketRealtimeConnection bucketRealtimeConnection = MockBucketRealtimeConnection;

                var datas = new List<TestBucketDataModel>(TestDatas);
                var newData = new TestBucketDataModel("new", "new");

                // ---

                webSocketClient.ConnectAsync(string.Empty, socket => new BucketRealtimeConnection(socket))
                    .ReturnsForAnyArgs(new UniTask<IWebSocketConnection>(bucketRealtimeConnection));
                bucketRealtimeConnection.Subscribe(Arg.Any<IObserver<ServerMessage>>()).Returns(delegate(CallInfo info)
                {
                    return datas.ObserveEveryValueChanged(list => list.Count).Skip(1).Select(list =>
                            new ServerMessage(DataChangeType.Insert, JsonConvert.SerializeObject(newData)))
                        .Subscribe(info.Arg<IObserver<ServerMessage>>());
                });

                // ---

                BucketConnection<TestBucketDataModel> bucketConnection =
                    await bucketService.Realtime.ConnectToBucketAsync<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());

                // ---

                CancellationTokenSource source = new CancellationTokenSource();
                bucketConnection.Subscribe(message =>
                {
                    Assert.IsTrue(message.Kind == DataChangeType.Insert);
                    source.Cancel();
                });

                // ---

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(
                    unit => datas.Add(newData)
                );

                await UniTask.WaitUntilCanceled(source.Token).Timeout(TimeSpan.FromSeconds(1));
            });


            [UnityTest]
            public IEnumerator InsertToBucket() => UniTask.ToCoroutine(async delegate()
            {
                CancellationTokenSource source = new CancellationTokenSource();

                (BucketService bucketService, IWebSocketClient webSocketClient) = MockBucketService;
                IBucketRealtimeConnection bucketRealtimeConnection = MockBucketRealtimeConnection;

                var datas = new List<TestBucketDataModel>(TestDatas);
                var newData = new TestBucketDataModel("new", "new");

                // ---

                webSocketClient.ConnectAsync(Arg.Any<string>(), socket => new BucketRealtimeConnection(socket))
                    .Returns(new UniTask<IWebSocketConnection>(bucketRealtimeConnection));
                WhenSentMessageThroughWebSocketDo(bucketRealtimeConnection, message => datas.Add(message.Item2));

                WhenWebSocketSubscribed(bucketRealtimeConnection, delegate(CallInfo info)
                {
                    return datas.ObserveEveryValueChanged(list => list.Count).Skip(1).Where(i => i > TestDatas.Length)
                        .Select(
                            list =>
                                new ServerMessage(DataChangeType.Insert,
                                    JsonConvert.SerializeObject(newData)))
                        .Subscribe(info.Arg<IObserver<ServerMessage>>());
                });
                // ---

                BucketConnection<TestBucketDataModel> bucketConnection =
                    await bucketService.Realtime.ConnectToBucketAsync<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());

                bucketConnection.Subscribe(message =>
                {
                    Assert.IsTrue(message.Kind == DataChangeType.Insert);
                    Assert.IsTrue(message.Document.Title == newData.Title &&
                                  message.Document.Description == newData.Description);
                    source.Cancel();
                });

                Observable.ReturnUnit().DelayFrame(5).Subscribe(unit => bucketConnection.Insert(newData));

                await UniTask.WaitUntilCanceled(source.Token).Timeout(TimeSpan.FromSeconds(1));
            });

            [UnityTest]
            public IEnumerator DeleteFromBucket() => UniTask.ToCoroutine(async delegate()
            {
                CancellationTokenSource source = new CancellationTokenSource();

                (BucketService bucketService, IWebSocketClient webSocketClient) = MockBucketService;
                IBucketRealtimeConnection bucketRealtimeConnection = MockBucketRealtimeConnection;

                var datas = new List<TestBucketDataModel>(TestDatas);
                var deletedData = datas[0];

                webSocketClient.ConnectAsync(String.Empty, socket => new BucketRealtimeConnection(socket))
                    .ReturnsForAnyArgs(new UniTask<IWebSocketConnection>(bucketRealtimeConnection));
                WhenSentMessageThroughWebSocketDo(bucketRealtimeConnection, message => datas.Remove(deletedData));

                WhenWebSocketSubscribed(bucketRealtimeConnection, delegate(CallInfo info)
                {
                    return datas.ObserveEveryValueChanged(list => list.Count).Skip(1).Where(i => i < TestDatas.Length)
                        .Select(i =>
                        {
                            return new ServerMessage(DataChangeType.Delete,
                                JsonConvert.SerializeObject(deletedData));
                        }).Subscribe(info.Arg<IObserver<ServerMessage>>());
                });

                BucketConnection<TestBucketDataModel> bucketConnection =
                    await bucketService.Realtime.ConnectToBucketAsync<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());

                bucketConnection.Subscribe(change =>
                {
                    Assert.IsTrue(change.Kind == DataChangeType.Delete);
                    Assert.IsTrue(change.Document.Title == deletedData.Title &&
                                  change.Document.Description == deletedData.Description);
                    source.Cancel();
                });

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(unit => bucketConnection.Delete(deletedData));

                await UniTask.WaitUntilCanceled(source.Token).Timeout(TimeSpan.FromSeconds(1));
            });

            [UnityTest]
            public IEnumerator PatchBucket() => UniTask.ToCoroutine(async delegate()
            {
                CancellationTokenSource source = new CancellationTokenSource();

                (BucketService bucketService, IWebSocketClient webSocketClient) = MockBucketService;
                IBucketRealtimeConnection bucketRealtimeConnection = MockBucketRealtimeConnection;

                var datas = new List<TestBucketDataModel>(TestDatas);
                var patchedData = datas[0];
                var newTitle = "patchedTitle";

                webSocketClient.ConnectAsync(Arg.Any<string>(), socket => new BucketRealtimeConnection(socket))
                    .Returns(new UniTask<IWebSocketConnection>(bucketRealtimeConnection));
                WhenSentMessageThroughWebSocketDo(bucketRealtimeConnection, tuple => datas[0].Title = newTitle);
                WhenWebSocketSubscribed(bucketRealtimeConnection, delegate(CallInfo info)
                {
                    return datas[0].ObserveEveryValueChanged(model => model.Title).Skip(1).Select(i =>
                    {
                        return new ServerMessage(DataChangeType.Update,
                            JsonConvert.SerializeObject(new TestBucketDataModel(newTitle, patchedData.Description)));
                    }).Subscribe(info.Arg<IObserver<ServerMessage>>());
                });

                BucketConnection<TestBucketDataModel> bucketConnection =
                    await bucketService.Realtime.ConnectToBucketAsync<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());

                bucketConnection.Subscribe(change =>
                {
                    Assert.IsTrue(change.Kind == DataChangeType.Update);
                    Assert.IsTrue(change.Document.Title == newTitle);
                    source.Cancel();
                });

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(unit => bucketConnection.Patch(patchedData));

                await UniTask.WaitUntilCanceled(source.Token).Timeout(TimeSpan.FromSeconds(1));
            });

            [UnityTest]
            public IEnumerator ReplaceBucket() => UniTask.ToCoroutine(async delegate
            {
                CancellationTokenSource source = new CancellationTokenSource();

                (BucketService bucketService, IWebSocketClient webSocketClient) = MockBucketService;
                IBucketRealtimeConnection bucketRealtimeConnection = MockBucketRealtimeConnection;

                var datas = new List<TestBucketDataModel>(TestDatas);
                var replacedData = new TestBucketDataModel("replacedTitle", "replacedDesc");

                webSocketClient.ConnectAsync(Arg.Any<string>(), socket => new BucketRealtimeConnection(socket))
                    .Returns(new UniTask<IWebSocketConnection>(bucketRealtimeConnection));
                WhenSentMessageThroughWebSocketDo(bucketRealtimeConnection, tuple => datas[0] = replacedData);
                WhenWebSocketSubscribed(bucketRealtimeConnection, delegate(CallInfo info)
                {
                    return datas.ObserveEveryValueChanged(list => list[0]).Skip(1).Select(i =>
                    {
                        return new ServerMessage(DataChangeType.Replace,
                            JsonConvert.SerializeObject(replacedData));
                    }).Subscribe(info.Arg<IObserver<ServerMessage>>());
                });

                BucketConnection<TestBucketDataModel> bucketConnection =
                    await bucketService.Realtime.ConnectToBucketAsync<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());

                bucketConnection.Subscribe(change =>
                {
                    Assert.IsTrue(change.Kind == DataChangeType.Replace);
                    Assert.IsTrue(change.Document.Title == replacedData.Title);
                    Assert.IsTrue(change.Document.Description == replacedData.Description);
                    source.Cancel();
                });

                Observable.NextFrame(FrameCountType.EndOfFrame)
                    .Subscribe(unit => bucketConnection.Replace(replacedData));

                await UniTask.WaitUntilCanceled(source.Token).Timeout(TimeSpan.FromSeconds(1));
            });

            [UnityTest]
            public IEnumerator CloseConnectionWhenStreamDisposed() => UniTask.ToCoroutine(async delegate()
            {
                CancellationTokenSource source = new CancellationTokenSource(1000);

                (BucketService bucketService, IWebSocketClient webSocketClient) = MockBucketService;
                IBucketRealtimeConnection bucketRealtimeConnection = MockBucketRealtimeConnection;

                webSocketClient.ConnectAsync(Arg.Any<string>(), socket => new BucketRealtimeConnection(socket))
                    .Returns(info => new UniTask<IWebSocketConnection>(bucketRealtimeConnection));

                BucketConnection<TestBucketDataModel> bucketConnection =
                    await bucketService.Realtime.ConnectToBucketAsync<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());

                bucketConnection.Dispose();
                bucketRealtimeConnection.Received().DisconnectAsync();

                await UniTask.WaitUntilCanceled(source.Token);
            });
        }
    }
}