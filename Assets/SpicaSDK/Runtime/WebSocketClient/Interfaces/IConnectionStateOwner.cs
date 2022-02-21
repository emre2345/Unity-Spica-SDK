using System;
using NativeWebSocket;

namespace SpicaSDK.Runtime.WebSocketClient.Interfaces
{
    public interface IConnectionStateOwner
    {
        IObservable<WebSocketState> ObserveState { get; }

        IObservable<WebSocketCloseCode> ObserveClose { get; }
    }
}