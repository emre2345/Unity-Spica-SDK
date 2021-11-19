using System;
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
            public readonly DataChangeType ChangeType;
            public readonly T Document;

            public BucketChange(DataChangeType changeType, T document)
            {
                ChangeType = changeType;
                Document = document;
            }
        }

        private IWebSocketConnection connection;
        private CompositeDisposable subscriptions;

        public BucketConnection(IWebSocketConnection connection)
        {
            this.connection = connection;

            subscriptions = new CompositeDisposable(16);
        }

        public IDisposable Subscribe(IObserver<BucketChange<T>> observer)
        {
            return connection.Do(message =>
            {
                if (message.ChangeType == DataChangeType.Response)
                {
                    //TODO: check status code and text
                }
            }).Where(message => message.ChangeType != DataChangeType.Response).Select(message =>
            {
                if (message.ChangeType == DataChangeType.Error)
                    return new BucketChange<T>(DataChangeType.Error, null);

                return new BucketChange<T>(message.ChangeType,
                    JsonConvert.DeserializeObject<T>(message.Text));
            }).Subscribe(observer).AddTo(subscriptions);
        }

        public void Insert(T document)
        {
            ApplyOperation(RealtimeMessageEvents.Insert, document);
        }

        public void Delete(T document)
        {
            ApplyOperation(RealtimeMessageEvents.Delete, document);
        }

        public void Patch(T document)
        {
            ApplyOperation(RealtimeMessageEvents.Patch, document);
        }

        private void ApplyOperation(string @event, T document)
        {
            (string @event, string data) message =
                (@event, JsonConvert.SerializeObject(document));

            connection.SendMessage(JsonConvert.SerializeObject(message));
        }

        public void Dispose()
        {
            subscriptions.Clear();
            connection.Disconnect();
        }
    }
}