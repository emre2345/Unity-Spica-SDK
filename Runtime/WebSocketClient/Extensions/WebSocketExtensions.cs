using System;
using NativeWebSocket;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using UniRx;
using IWebSocket = SpicaSDK.Runtime.WebSocketClient.Interfaces.IWebSocket;

namespace SpicaSDK.Services.WebSocketClient.Extensions
{
    public static class WebSocketExtensions
    {
        public static void Reconnect(this IConnectable connectable, Predicate<WebSocketCloseCode> condition)
        {
            connectable.ObserveClose.Where(code => condition(code))
                .Select(code => connectable.ObserveState.First(state => state == WebSocketState.Closed)).Switch()
                .Subscribe(code =>
                {
                    SpicaLogger.Instance.Log($"[{connectable.GetType().Name}] Reconnecting");
                    connectable.Connect();
                });
        }
    }
}