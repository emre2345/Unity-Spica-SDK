using System;
using Cysharp.Threading.Tasks;
using SpicaSDK.Services.WebSocketClient;

namespace SpicaSDK.Interfaces
{
    public interface IWebSocketClient : IDisposable
    {
        UniTask<IWebSocketConnection> ConnectAsync(string url);
    }

    public interface IWebSocketConnection : IObservable<ServerMessage>
    {
        UniTask DisconnectAsync();

        UniTask SendMessageAsync(string message);
    }
}