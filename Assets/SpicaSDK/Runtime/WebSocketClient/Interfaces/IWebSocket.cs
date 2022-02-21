using System;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using UniRx;

namespace SpicaSDK.Runtime.WebSocketClient.Interfaces
{
    public interface IWebSocket : IConnectable
    {
        IObservable<Unit> ObserveOpen { get; }

        IObservable<string> ObserveError { get; }
        
        IObservable<string> ObserveMessage { get; }
        
        WebSocketState State { get; }
        
        UniTask Close();

        UniTask SendText(string message);
    }
}