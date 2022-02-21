using System;
using NativeWebSocket;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using UniRx;
using IWebSocket = SpicaSDK.Runtime.WebSocketClient.Interfaces.IWebSocket;

namespace SpicaSDK.Services.WebSocketClient.Extensions
{
    public static class WebSocketExtensions
    {
        public static void Reconnect(this IReconnectable reconnectable, Predicate<WebSocketCloseCode> condition)
        {
            reconnectable.ObserveClose.Where(code => condition(code)).DelayFrame(1).Subscribe(code => reconnectable.Connect());
        }
    }
}