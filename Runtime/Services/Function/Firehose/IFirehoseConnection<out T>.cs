using System;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;

namespace SpicaSDK.Runtime.Services.Function.Firehose
{
    public interface IFirehoseConnection<out T> : IWebSocketConnection, IObservable<T>
    {
    }
}