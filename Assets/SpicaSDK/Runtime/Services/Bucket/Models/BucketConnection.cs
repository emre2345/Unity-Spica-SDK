using System;
using System.Net;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using SpicaSDK.Runtime.Services.Bucket.Realtime;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using UniRx;

namespace SpicaSDK.Services.Models
{
    public class BucketConnection<T> : IDisposable,
        IObservable<BucketConnection<T>.BucketChange<T>> where T : class
    {
        public struct BucketChange<T>
        {
            public readonly DataChangeType Kind;
            public readonly T Document;
            public readonly string Message;
            public HttpStatusCode StatusCode;

            public BucketChange(DataChangeType changeType, T document, HttpStatusCode statusCode = HttpStatusCode.OK,
                string message = "")
            {
                Kind = changeType;
                Document = document;
                Message = message;
                StatusCode = statusCode;
            }
        }

        private IBucketRealtimeConnection connection;
        private CompositeDisposable subscriptions;

        public BucketConnection(IBucketRealtimeConnection connection)
        {
            this.connection = connection;

            subscriptions = new CompositeDisposable(16);
        }

        public IDisposable Subscribe(IObserver<BucketChange<T>> observer)
        {
            return connection.Where(message =>
                    message.Kind != DataChangeType.Response && message.Kind != DataChangeType.EndOfInitial)
                .Select(message =>
                {
                    if (message.Kind == DataChangeType.Error)
                        return new BucketChange<T>(DataChangeType.Error, null);

                    return new BucketChange<T>(message.Kind,
                        message.Document.ToObject<T>());
                }).Subscribe(observer).AddTo(subscriptions);
        }

        public IObservable<BucketChange<T>> Insert(T document)
        {
            ApplyOperationAsync(RealtimeMessageEvents.Insert, document);
            return connection.Where(message => message.Kind == DataChangeType.Response).First().Select(message =>
                new BucketChange<T>(DataChangeType.Response, null,
                    (HttpStatusCode)message.Status, message.Message));
        }

        public IObservable<BucketChange<T>> Delete(T document)
        {
            ApplyOperationAsync(RealtimeMessageEvents.Delete, document);
            return connection.Where(message => message.Kind == DataChangeType.Response).First().Select(message =>
                new BucketChange<T>(DataChangeType.Response, null,
                    (HttpStatusCode)message.Status, message.Message));
        }

        public IObservable<BucketChange<T>> Patch(T document)
        {
            ApplyOperationAsync(RealtimeMessageEvents.Patch, document);
            return connection.Where(message => message.Kind == DataChangeType.Response).First().Select(message =>
                new BucketChange<T>(DataChangeType.Response, null,
                    (HttpStatusCode)message.Status, message.Message));
        }

        public IObservable<BucketChange<T>> Replace(T document)
        {
            ApplyOperationAsync(RealtimeMessageEvents.Replace, document);
            return connection.Where(message => message.Kind == DataChangeType.Response).First().Select(message =>
                new BucketChange<T>(DataChangeType.Response, null,
                    (HttpStatusCode)message.Status, message.Message));
        }

        private async UniTask ApplyOperationAsync(string @event, T document)
        {
            var message = new { @event = @event, data = document };

            await connection.SendMessageAsync(JsonConvert.SerializeObject(message));
        }

        public void ReconnectWhen(Predicate<WebSocketCloseCode> condition) => connection.ReconnectWhen(condition);

        public void Dispose()
        {
            subscriptions.Clear();
            connection.DisconnectAsync();
        }
    }
}