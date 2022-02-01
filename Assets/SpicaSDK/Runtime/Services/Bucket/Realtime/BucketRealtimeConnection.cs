using System;
using NativeWebSocket;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using SpicaSDK.Runtime.Services.Bucket.Realtime;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using UniRx;

namespace SpicaSDK.Services.WebSocketClient
{
    public class BucketRealtimeConnection : WebSocketConnection, IBucketRealtimeConnection
    {
        public BucketRealtimeConnection(WebSocket socket) : base(socket)
        {
        }

        public IDisposable Subscribe(IObserver<ServerMessage> observer)
        {
            return observeMessage
                .Select(s => JsonConvert.DeserializeObject<ServerMessage>(s)).Subscribe(observer).AddTo(subscriptions);
        }
    }
}