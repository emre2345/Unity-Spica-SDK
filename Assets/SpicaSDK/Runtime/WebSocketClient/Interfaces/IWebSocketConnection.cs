using System;
using Cysharp.Threading.Tasks;
using NativeWebSocket;

namespace SpicaSDK.Runtime.WebSocketClient.Interfaces
{
    public interface IWebSocketConnection : IConnectionStateOwner
    {
        void ReconnectWhen(Predicate<WebSocketCloseCode> condition);
        
        UniTask DisconnectAsync();

        UniTask SendMessageAsync(string message);
    }

}