using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SpicaSDK.Runtime.Services.Bucket.Realtime;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using UniRx;

namespace SpicaSDK.Services.WebSocketClient
{
    public class BucketRealtimeConnection : WebSocketConnection, IBucketRealtimeConnection
    {
        private Subject<string> message;
        private IDisposable socketSubscription;

        public BucketRealtimeConnection(IWebSocket socket) : base(socket)
        {
            message = new Subject<string>();
            socketSubscription = socket.ObserveMessage.Subscribe(message);
        }

        public IDisposable Subscribe(IObserver<ServerMessage> observer)
        {
            return message
                // .Select(s => JsonConvert.DeserializeObject<ServerMessage>(s)).Subscribe(observer).AddTo(subscriptions);
                .Select(s => JsonConvert.DeserializeObject<ServerMessage>(s)).Subscribe(observer);
        }

        public override UniTask DisconnectAsync()
        {
            socketSubscription.Dispose();
            
            return base.DisconnectAsync();
        }
    }
}