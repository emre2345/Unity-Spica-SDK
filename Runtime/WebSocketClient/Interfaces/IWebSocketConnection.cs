using System;
using Cysharp.Threading.Tasks;
using NativeWebSocket;

namespace SpicaSDK.Runtime.WebSocketClient.Interfaces
{
    public interface IWebSocketConnection : IConnectionStateOwner
    {
        UniTask DisconnectAsync();

        UniTask SendMessageAsync(string message);
    }

    public interface IConnectionStateOwner
    {
        IObservable<WebSocketState> ObserveState { get; }

        IObservable<WebSocketCloseCode> ObserveConnectionClose { get; }
    }
}