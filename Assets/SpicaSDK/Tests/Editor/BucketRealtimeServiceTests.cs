using System;
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
            public IEnumerator WatchDocument() => UniTask.ToCoroutine(async delegate()
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

                SkipInitialData(documentConnection).Subscribe(message =>
                {
                    Assert.IsTrue(message.Title.Equals(newTitle));
                });

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(unit => firstData.Title = newTitle);
            });

            private IObservable<T> SkipInitialData<T>(IObservable<T> stream) =>
                stream.Skip(1);


            [UnityTest]
            public IEnumerator WatchBucket() => UniTask.ToCoroutine(async delegate()
            {
                IWebSocketClient webSocketClient;
                BucketConnection<TestBucketDataModel> bucketConnection = GetBucketConnection(out webSocketClient);

                var datas = new List<TestBucketDataModel>(TestDatas);
                var newData = new TestBucketDataModel("new", "new");

                webSocketClient.Connect(string.Empty).ReturnsForAnyArgs(delegate(CallInfo info)
                {
                    return datas.ObserveEveryValueChanged(list => list).Select(list =>
                        new Message(DataChangeType.Insert, HttpStatusCode.OK, JsonConvert.SerializeObject(newData)));
                });

                datas.Add(newData);

                SkipInitialData(bucketConnection).Subscribe(message =>
                {
                    Assert.IsTrue(message.ChangeType == DataChangeType.Insert);
                });
                //
                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(
                    unit => datas = new List<TestBucketDataModel>(datas)
                );
            });

            [UnityTest]
            public IEnumerator InsertToBucket() => UniTask.ToCoroutine(async delegate() { });

            [UnityTest]
            public IEnumerator DeleteFromBucket() => UniTask.ToCoroutine(async delegate() { });

            [UnityTest]
            public IEnumerator PatchBucket() => UniTask.ToCoroutine(async delegate() { });

            [UnityTest]
            public IEnumerator CloseConnectionWhenStreamDisposed() => UniTask.ToCoroutine(async delegate() { });

            private BucketConnection<TestBucketDataModel> GetBucketConnection(out IWebSocketClient webSocketClient)
            {
                ISpicaServer server = Substitute.For<ISpicaServer>();
                IHttpClient httpClient = Substitute.For<IHttpClient>();
                webSocketClient = MockWebSocketClient;

                BucketService bucketService = new BucketService(server, httpClient, webSocketClient);
                return
                    bucketService.Realtime.ConnectToBucket<TestBucketDataModel>(new Id(TestBucketId),
                        new QueryParams());
            }
        }
    }
}