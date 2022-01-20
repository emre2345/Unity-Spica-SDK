using System;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using SpicaSDK.Services.WebSocketClient;

namespace SpicaSDK.Interfaces
{
    public interface IWebSocketClient : IDisposable
    {
        UniTask<IWebSocketConnection> ConnectAsync(string url, Func<WebSocket, IWebSocketConnection> connectionFactory);
    }

    public interface IWebSocketConnection
    {
        UniTask Connected();

        UniTask DisconnectAsync();

        UniTask SendMessageAsync(string message);
    }

    public interface IBucketRealtimeConnection : IWebSocketConnection, IObservable<ServerMessage>
    {
    }

    public interface IFirehoseConnection<out T> : IWebSocketConnection, IObservable<T>
    {
    }
}