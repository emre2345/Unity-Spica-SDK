using System;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using SpicaSDK.Services.Models;
using SpicaSDK.Services.WebSocketClient;
using UniRx;

namespace SpicaSDK.Services
{
    public partial class BucketService
    {
        public readonly RealtimeService Realtime;

        public class RealtimeService : ISpicaService
        {
            private ISpicaServer server;
            private IWebSocketClient webSocketClient;

            public RealtimeService(ISpicaServer server, IWebSocketClient webSocketClient)
            {
                this.server = server;
                this.webSocketClient = webSocketClient;
            }

            public IObservable<T> Get<T>(Id bucketId, Id documentId)
            {
                string url = server.BucketDataDocumentUrl(bucketId, documentId).Replace("http", "ws");
                IObservable<Message> connection = webSocketClient.Connect(url);
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
                            return default(T);
                    }
                });
            }
        }
    }
}