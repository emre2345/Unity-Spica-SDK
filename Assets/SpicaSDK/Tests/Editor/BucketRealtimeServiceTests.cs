using System;
using System.Collections;
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
            public IEnumerator Get() => UniTask.ToCoroutine(async delegate()
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
                IObservable<TestBucketDataModel> stream =
                    bucketService.Realtime.Get<TestBucketDataModel>(new Id(TestBucketId), firstData.Id);

                string newTitle = "newTitle";

                SkipInitialData(stream).Subscribe(message => { Assert.IsTrue(message.Title.Equals(newTitle)); });

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(unit => firstData.Title = newTitle);
            });

            private IObservable<TestBucketDataModel> SkipInitialData(IObservable<TestBucketDataModel> stream) =>
                stream.Skip(1);

            [UnityTest]
            public IEnumerator GetAll() => UniTask.ToCoroutine(async delegate() { });
        }
    }
}