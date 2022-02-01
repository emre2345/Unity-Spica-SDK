using System;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using SpicaSDK.Services.WebSocketClient;

namespace SpicaSDK.Runtime.Services.Bucket.Realtime
{
    public interface IBucketRealtimeConnection : IWebSocketConnection, IObservable<ServerMessage>
    {
    }
}