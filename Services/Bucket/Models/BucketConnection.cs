using System;
using Newtonsoft.Json;
using SpicaSDK.Services.WebSocketClient;
using UniRx;

namespace SpicaSDK.Services.Models
{
    public class BucketConnection<T> : IObservable<BucketConnection<T>.BucketChange<T>> where T : class
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

        private IObservable<Message> connection;
        private Action<string> sendMessage;

        public BucketConnection(IObservable<Message> connection, Action<string> sendMessage)
        {
            this.connection = connection;
            this.sendMessage = sendMessage;
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
            }).Subscribe(observer);
        }

        public void Insert(T document)
        {
            (string @event, string data) message =
                (RealtimeMessageEvents.Insert, JsonConvert.SerializeObject(document));

            sendMessage(JsonConvert.SerializeObject(message));
        }
    }
}