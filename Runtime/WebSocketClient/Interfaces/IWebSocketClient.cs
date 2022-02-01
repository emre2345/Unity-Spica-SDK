using System;
using Cysharp.Threading.Tasks;
using NativeWebSocket;

namespace SpicaSDK.Runtime.WebSocketClient.Interfaces
{
    public interface IWebSocketClient : IDisposable
    {
        UniTask<IWebSocketConnection> ConnectAsync(string url, Func<WebSocket, IWebSocketConnection> connectionFactory);
    }
}