using System;
using Cysharp.Threading.Tasks;

namespace SpicaSDK.Runtime.WebSocketClient.Interfaces
{
    public interface IWebSocketClient : IDisposable
    {
        UniTask<IWebSocketConnection> ConnectAsync(string url, Func<IWebSocket, IWebSocketConnection> connectionFactory);
    }
}