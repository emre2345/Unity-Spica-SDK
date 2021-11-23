using System;
using System.Net;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using SpicaSDK.Services.WebSocketClient;
using UniRx;

namespace SpicaSDK.Services.Models
{
    public class BucketConnection<T> : IDisposable, IObservable<BucketConnection<T>.BucketChange<T>> where T : class
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

        private IWebSocketConnection connection;
        private CompositeDisposable subscriptions;
        private Subject<BucketChange<T>> serverResponse;

        public BucketConnection(IWebSocketConnection connection)
        {
            this.connection = connection;

            subscriptions = new CompositeDisposable(16);
        }

        public IDisposable Subscribe(IObserver<BucketChange<T>> observer)
        {
            return connection.Where(delegate(ServerMessage message)
            {
                if (message.Kind == DataChangeType.Response)
                {
                    serverResponse?.OnNext(new BucketChange<T>(DataChangeType.Response, null,
                        (HttpStatusCode)message.Status, message.Message));
                    return false;
                }

                return message.Kind != DataChangeType.EndOfInitial;
            }).Select(message =>
            {
                if (message.Kind == DataChangeType.Error)
                    return new BucketChange<T>(DataChangeType.Error, null);

                return new BucketChange<T>(message.Kind,
                    message.Document.ToObject<T>());
            }).Subscribe(observer).AddTo(subscriptions);
        }

        public IObservable<BucketChange<T>> Insert(T document)
        {
            ApplyOperation(RealtimeMessageEvents.Insert, document);
            return serverResponse = new Subject<BucketChange<T>>();
        }

        public IObservable<BucketChange<T>> Delete(T document)
        {
            ApplyOperation(RealtimeMessageEvents.Delete, document);
            return serverResponse = new Subject<BucketChange<T>>();
        }

        public IObservable<BucketChange<T>> Patch(T document)
        {
            ApplyOperation(RealtimeMessageEvents.Patch, document);
            return serverResponse = new Subject<BucketChange<T>>();
        }

        public IObservable<BucketChange<T>> Replace(T document)
        {
            ApplyOperation(RealtimeMessageEvents.Replace, document);
            return serverResponse = new Subject<BucketChange<T>>();
        }

        private void ApplyOperation(string @event, T document)
        {
            var message = new { @event = @event, data = document };

            connection.SendMessage(JsonConvert.SerializeObject(message));
        }

        public void Dispose()
        {
            subscriptions.Clear();
            connection.Disconnect();
        }
    }
}