using System;
using UniRx;
using System.Collections;
using System.Net;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
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

            private IHttpClient httpClient;
            private IWebSocketClient webSocketClient;
            private ISpicaServer spicaServer;
            private BucketService bucketService;

            [SetUp]
            public void Setup()
            {
                httpClient = new HttpClient();
                spicaServer = new SpicaServer(url, httpClient);
                webSocketClient = new WebSocketClient();
                bucketService = new BucketService(spicaServer, httpClient, webSocketClient);
            }

            private async UniTask Login()
            {
                IdentityService identityService = new IdentityService(spicaServer, httpClient);
                spicaServer.Identity = await identityService.LogInAsync("spica", "spica", float.MaxValue);
            }

            public void Cleanup()
            {
                disposable.Clear();
                webSocketClient?.Dispose();
            }

            [UnityTest]
            public IEnumerator WatchDocument() => UniTask.ToCoroutine(async delegate()
            {
                await UniTask.Delay(1);
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                await Login();

                var queryParams = new QueryParams();
                queryParams.AddQuery("limit", "1");

                var newTitle = "updatedTitle";

                Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
                Bucket newBucket = await bucketService.CreateAsync(bucket);

                var data = await bucketService.Data.InsertAsync<TestBucketDataModel>(new Id(newBucket.Id),
                    new TestBucketDataModel("newTitle", "newDesc"));
                var documentWatch =
                    await bucketService.Realtime.WatchDocumentAsync<TestBucketDataModel>(new Id(newBucket.Id), data.Id);

                documentWatch.Do(model => Debug.Log(model.ToString())).Skip(1).Subscribe(
                    delegate(TestBucketDataModel model)
                    {
                        Assert.AreEqual(newTitle, model.Title);
                        cancellationTokenSource.Cancel();
                    }).AddTo(disposable);

                await UniTask.Delay(1);
                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(delegate(UniRx.Unit unit)
                {
                    bucketService.Data.ReplaceAsync(new Id(newBucket.Id), data.Id,
                        new TestBucketDataModel(newTitle, data.Description));
                });

                await UniTask.WaitUntilCanceled(cancellationTokenSource.Token)
                    .Timeout(TimeSpan.FromSeconds(5));

                await bucketService.DeleteAsync(new Id(newBucket.Id));
            });

            [UnityTest]
            public IEnumerator WatchDocumentDisconnectsWithDeletion() => UniTask.ToCoroutine(async delegate()
            {
                await UniTask.Delay(1);
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                await Login();

                Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
                Bucket newBucket = await bucketService.CreateAsync(bucket);

                var data = await bucketService.Data
                    .InsertAsync<TestBucketDataModel>(new Id(newBucket.Id), new TestBucketDataModel("q", "q2"));
                var documentWatch =
                    await bucketService.Realtime.WatchDocumentAsync<TestBucketDataModel>(new Id(newBucket.Id), data.Id);

                documentWatch.Subscribe(model => { },
                    () =>
                    {
                        Debug.Log("Subscription completed automatically because of deletion");
                        disposable.Clear();
                        cancellationTokenSource.Cancel();
                    }).AddTo(disposable);


                await UniTask.Delay(1);
                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(delegate(UniRx.Unit unit)
                {
                    bucketService.Data.RemoveAsync(new Id(newBucket.Id), data.Id);
                });

                await UniTask.WaitUntilCanceled(cancellationTokenSource.Token).Timeout(TimeSpan.FromSeconds(5));
                await bucketService.DeleteAsync(new Id(newBucket.Id));
            });

            [UnityTest]
            public IEnumerator ConnectionDropsIfInitialNotReceived() => UniTask.ToCoroutine(async delegate()
            {
                await UniTask.Delay(1);
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                await Login();

                Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
                Bucket newBucket = await bucketService.CreateAsync(bucket);

                var documentWatch =
                    await bucketService.Realtime.WatchDocumentAsync<TestBucketDataModel>(new Id(newBucket.Id),
                        new Id("nonExistingId"));

                documentWatch.Subscribe(model => { },
                    () =>
                    {
                        Debug.Log("Subscription dropped automatically because of non existing data");
                        disposable.Clear();
                        cancellationTokenSource.Cancel();
                    }).AddTo(disposable);

                await UniTask.WaitUntilCanceled(cancellationTokenSource.Token).Timeout(TimeSpan.FromSeconds(5));
                await bucketService.DeleteAsync(new Id(newBucket.Id));
            });

            [UnityTest]
            public IEnumerator WatchBucket() => UniTask.ToCoroutine(async delegate()
            {
                await UniTask.Delay(1);

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                await Login();

                Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
                Bucket newBucket = await bucketService.CreateAsync(bucket);


                var bucketConnection =
                    await bucketService.Realtime.ConnectToBucketAsync<TestBucketDataModel>(new Id(newBucket.Id),
                        new QueryParams());

                bucketConnection.Where(change => change.Kind != DataChangeType.Initial).Subscribe(change =>
                {
                    Assert.AreEqual(DataChangeType.Insert, change.Kind);
                    disposable.Clear();
                    cancellationTokenSource.Cancel();
                }).AddTo(disposable);

                await UniTask.Delay(1);
                Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(unit =>
                    bucketService.Data.InsertAsync(new Id(newBucket.Id), new TestBucketDataModel("t1", "d1")));

                disposable.Add(bucketConnection);

                await UniTask.WaitUntilCanceled(cancellationTokenSource.Token).Timeout(TimeSpan.FromSeconds(5));
                await bucketService.DeleteAsync(new Id(newBucket.Id));
            });

            [UnityTest]
            public IEnumerator InsertToBucket() => UniTask.ToCoroutine(async delegate()
            {
                await UniTask.Delay(1);
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                await Login();

                Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
                Bucket newBucket = await bucketService.CreateAsync(bucket);


                var bucketConnection =
                    await bucketService.Realtime.ConnectToBucketAsync<TestBucketDataModel>(new Id(newBucket.Id),
                        new QueryParams());

                var task1 = bucketConnection.Where(change => change.Kind != DataChangeType.Initial).Do(
                    change =>
                    {
                        Assert.AreEqual(DataChangeType.Insert, change.Kind);
                        Assert.AreEqual("socket1", change.Document.Title);
                    }).First().ToUniTask();

                await UniTask.Delay(1);
                var serverResponse = bucketConnection.Insert(new TestBucketDataModel("socket1", "socketDesc1")).Do(
                    change =>
                    {
                        Assert.AreEqual(DataChangeType.Response, change.Kind);
                        Assert.AreEqual(HttpStatusCode.Created, change.StatusCode);
                    }).First().ToUniTask();

                UniTask.WhenAll(task1, serverResponse).ContinueWith(tuple =>
                {
                    disposable.Clear();
                    cancellationTokenSource.Cancel();
                });

                disposable.Add(bucketConnection);

                await UniTask.WaitUntilCanceled(cancellationTokenSource.Token).Timeout(TimeSpan.FromSeconds(5));

                await bucketService.DeleteAsync(new Id(newBucket.Id));
            });
        }
    }
}