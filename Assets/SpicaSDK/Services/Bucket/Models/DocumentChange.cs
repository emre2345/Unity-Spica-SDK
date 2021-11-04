using System;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using SpicaSDK.Services.WebSocketClient;
using UniRx;

namespace SpicaSDK.Services.Models
{
    public class DocumentChange<T> : IObservable<T> where T : class
    {
        private IObservable<Message> connection;

        public DocumentChange(IObservable<Message> connection)
        {
            this.connection = connection;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return connection.Do(message =>
            {
                if (message.ChangeType == DataChangeType.Response)
                {
                    //TODO: check status code and text
                }
            }).Where(message => message.ChangeType != DataChangeType.Response).Select(message =>
            {
                switch (message.ChangeType)
                {
                    case DataChangeType.Initial:
                    case DataChangeType.Insert:
                    case DataChangeType.Replace:
                    case DataChangeType.Update:
                        return JsonConvert.DeserializeObject<T>(message.Text);
                    default:
                        return null;
                }
            }).Subscribe(observer);
        }
    }
}