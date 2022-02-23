using System;
using System.Collections;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SpicaSDK.Interfaces;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
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
                spicaServer = new SpicaServer(new SpicaServerUrl(url), httpClient);
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
                queryParams.Add("limit", "1");

                var newTitle = "updatedTitle";

                Bucket bucket = JsonConvert.DeserializeObject<Bucket>(TestBucketDataAsJson);
                Bucket newBucket = await bucketService.CreateAsync(bucket);

                var data = await bucketService.Data.InsertAsync<TestBucketDataModel>(new Id(newBucket.Id),
                    new TestBucketDataModel("newTitle", "newDesc"));
                var documentWatch =
                    await bucketService.Realtime.WatchDocumentAsync<TestBucketDataModel>(new Id(newBucket.Id), data.Id);

                disposable.Add(documentWatch.Do(model => Debug.Log(model.ToString())).Skip(1).Subscribe(
                    delegate(TestBucketDataModel model)
                    {
                        Assert.AreEqual(newTitle, model.Title);
                        cancellationTokenSource.Cancel();
                    }));

                await UniTask.Delay(1);
                UniTask.NextFrame().ToObservable().Subscribe(_ =>
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

                disposable.Add(documentWatch.Subscribe(model => { },
                    () =>
                    {
                        Debug.Log("Subscription completed automatically because of deletion");
                        disposable.Clear();
                        cancellationTokenSource.Cancel();
                    }));


                await UniTask.Delay(1);
                UniTask.NextFrame(PlayerLoopTiming.LastUpdate).ToObservable().Subscribe(_ =>
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

                disposable.Add(documentWatch.Subscribe(model => { },
                    () =>
                    {
                        Debug.Log("Subscription dropped automatically because of non existing data");
                        disposable.Clear();
                        cancellationTokenSource.Cancel();
                    }));

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

                disposable.Add(bucketConnection.Where(change => change.Kind != DataChangeType.Initial).Subscribe(
                    change =>
                    {
                        Assert.AreEqual(DataChangeType.Insert, change.Kind);
                        disposable.Clear();
                        cancellationTokenSource.Cancel();
                    }));

                await UniTask.Delay(1);
                UniTask.NextFrame(PlayerLoopTiming.Update).ToObservable().Subscribe(unit =>
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
                    }).FirstAsync().ToUniTask();

                await UniTask.NextFrame();
                var serverResponse = bucketConnection.Insert(new TestBucketDataModel("socket1", "socketDesc1")).Do(
                    change =>
                    {
                        Assert.AreEqual(DataChangeType.Response, change.Kind);
                        Assert.AreEqual(HttpStatusCode.Created, change.StatusCode);
                    }).FirstAsync().ToUniTask();

                UniTask.WhenAll(task1, serverResponse).ContinueWith(_ =>
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