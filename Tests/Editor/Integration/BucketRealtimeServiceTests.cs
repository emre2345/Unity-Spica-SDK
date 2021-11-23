using System;
using UniRx;
using System.Collections;
using System.Net;
using System.Threading;
using Cysharp.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SpicaSDK.Interfaces;
using SpicaSDK.Services;
using SpicaSDK.Services.Models;
using SpicaSDK.Services.Services.Identity;
using SpicaSDK.Services.WebSocketClient;
using UnityEngine;
using UnityEngine.TestTools;

namespace SpicaSDK.Tests.Editor.Integration
{
    public partial class BucketServiceTests
    {
        public class BucketRealtimeServiceTests : IPostBuildCleanup
        {
            private CompositeDisposable disposable = new CompositeDisposable();

            private async UniTask<(BucketService, IWebSocketClient)> Setup()
            {
                IHttpClient httpClient = new HttpClient();
                ISpicaServer spicaServer = new SpicaServer(url, httpClient);
                IWebSocketClient webSocketClient = new WebSocketClient();

                IdentityService identityService = new IdentityService(spicaServer, httpClient);
                spicaServer.Identity = await identityService.LogIn("spica", "spica", float.MaxValue);

                var service = new BucketService(spicaServer, httpClient, webSocketClient);
                return (service, webSocketClient);
            }

            [UnityTest]
            public IEnumerator WatchDocument() => UniTask.ToCoroutine(async delegate()
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                (BucketService bucketService, IWebSocketClient webSocketClient) = await Setup();

                var queryParams = new QueryParams();
                queryParams.AddQuery("limit", "1");

                var newTitle = "updatedTitle";

                var data = await bucketService.Data.Insert<TestBucketDataModel>(new Id(testBucketId),
                    new TestBucketDataModel("newTitle", "newDesc"));
                var documentWatch =
                    bucketService.Realtime.WatchDocument<TestBucketDataModel>(new Id(testBucketId), data.Id);
                documentWatch.Do(model => Debug.Log(model.ToString())).Skip(1).Subscribe(
                    delegate(TestBucketDataModel model)
                    {
                        Assert.AreEqual(newTitle, model.Title);
                        cancellationTokenSource.Cancel();
                    }).AddTo(disposable);

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(delegate(UniRx.Unit unit)
                {
                    bucketService.Data.Replace(new Id(testBucketId), data.Id,
                        new TestBucketDataModel(newTitle, data.Description));
                });

                await UniTask.WaitUntilCanceled(cancellationTokenSource.Token)
                    .TimeoutWithoutException(TimeSpan.FromSeconds(5))
                    .ContinueWith(b => Cleanup());
            });

            [UnityTest]
            public IEnumerator WatchDocumentDisconnectsWithDeletion() => UniTask.ToCoroutine(async delegate()
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                (BucketService bucketService, IWebSocketClient webSocketClient) = await Setup();

                var data = await bucketService.Data
                    .Insert<TestBucketDataModel>(new Id(testBucketId), new TestBucketDataModel("q", "q2"));
                var documentWatch =
                    bucketService.Realtime.WatchDocument<TestBucketDataModel>(new Id(testBucketId), data.Id);

                documentWatch.Subscribe(model => { },
                    () =>
                    {
                        Debug.Log("Subscription completed automatically because of deletion");
                        cancellationTokenSource.Cancel();
                    }).AddTo(disposable);


                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(delegate(UniRx.Unit unit)
                {
                    bucketService.Data.Remove(new Id(testBucketId), data.Id);
                });

                await UniTask.WaitUntilCanceled(cancellationTokenSource.Token)
                    .TimeoutWithoutException(TimeSpan.FromSeconds(5)).ContinueWith(b => Cleanup());
            });

            [UnityTest]
            public IEnumerator ConnectionDropsIfInitialNotReceived() => UniTask.ToCoroutine(async delegate()
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                (BucketService bucketService, IWebSocketClient webSocketClient) = await Setup();

                var documentWatch =
                    bucketService.Realtime.WatchDocument<TestBucketDataModel>(new Id(testBucketId),
                        new Id("nonExistingId"));

                documentWatch.Subscribe(model => { },
                    () =>
                    {
                        Debug.Log("Subscription dropped automatically because of non existing data");
                        disposable.Clear();
                        cancellationTokenSource.Cancel();
                    }).AddTo(disposable);

                await UniTask.WaitUntilCanceled(cancellationTokenSource.Token)
                    .TimeoutWithoutException(TimeSpan.FromSeconds(5)).ContinueWith(b => Cleanup());
            });

            [UnityTest]
            public IEnumerator WatchBucket() => UniTask.ToCoroutine(async delegate()
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                (BucketService bucketService, IWebSocketClient webSocketClient) = await Setup();

                var bucketConnection =
                    bucketService.Realtime.ConnectToBucket<TestBucketDataModel>(new Id(testBucketId),
                        new QueryParams());

                bucketConnection.Where(change => change.Kind != DataChangeType.Initial).Subscribe(change =>
                {
                    Assert.AreEqual(DataChangeType.Insert, change.Kind);
                    cancellationTokenSource.Cancel();
                }).AddTo(disposable);

                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(unit =>
                    bucketService.Data.Insert(new Id(testBucketId), new TestBucketDataModel("t1", "d1")));

                disposable.Add(bucketConnection);

                UniTask.Delay(TimeSpan.FromSeconds(5)).ContinueWith(() =>
                {
                    Cleanup();
                    cancellationTokenSource.Cancel();
                    Assert.Fail();
                });
                await UniTask.WaitUntilCanceled(cancellationTokenSource.Token);
            });

            [UnityTest]
            public IEnumerator InsertToBucket() => UniTask.ToCoroutine(async delegate()
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                (BucketService bucketService, IWebSocketClient webSocketClient) = await Setup();

                var bucketConnection =
                    bucketService.Realtime.ConnectToBucket<TestBucketDataModel>(new Id(testBucketId),
                        new QueryParams());

                var bucketSubscription = bucketConnection.Where(change => change.Kind != DataChangeType.Initial).Do(
                    change =>
                    {
                        Assert.AreEqual(DataChangeType.Insert, change.Kind);
                        Assert.AreEqual("socket1", change.Document.Title);
                    }).First().AsUnitObservable();

                var serverResponse = Observable.NextFrame(FrameCountType.EndOfFrame).Select(unit =>
                    bucketConnection.Insert(new TestBucketDataModel("socket1", "socketDesc1"))).Switch().Do(
                    delegate(BucketConnection<TestBucketDataModel>.BucketChange<TestBucketDataModel> change)
                    {
                        Assert.AreEqual(DataChangeType.Response, change.Kind);
                        Assert.AreEqual(HttpStatusCode.Created, change.StatusCode);
                    }).First().AsUnitObservable();

                Observable.WhenAll(serverResponse, bucketSubscription)
                    .Subscribe(unit => { cancellationTokenSource.Cancel(); }).AddTo(disposable);

                disposable.Add(bucketConnection);

                UniTask.Delay(TimeSpan.FromSeconds(5)).ContinueWith(() =>
                {
                    Cleanup();
                    cancellationTokenSource.Cancel();
                    Assert.Fail();
                });

                await UniTask.WaitUntilCanceled(cancellationTokenSource.Token);
            });

            public void Cleanup()
            {
                disposable.Clear();
            }
        }
    }
}