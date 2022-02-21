using System;
using Newtonsoft.Json;
using SpicaSDK.Runtime.Services.Bucket.Realtime;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using UniRx;

namespace SpicaSDK.Services.WebSocketClient
{
    public class BucketRealtimeConnection : WebSocketConnection, IBucketRealtimeConnection
    {
        public BucketRealtimeConnection(IWebSocket socket) : base(socket)
        {
        }

        public IDisposable Subscribe(IObserver<ServerMessage> observer)
        {
            return socket.ObserveMessage
                .Select(s => JsonConvert.DeserializeObject<ServerMessage>(s)).Subscribe(observer).AddTo(subscriptions);
        }
    }
}