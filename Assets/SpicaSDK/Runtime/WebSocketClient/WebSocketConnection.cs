using System;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using SpicaSDK.Services.WebSocketClient.Extensions;
using UniRx;
using IWebSocket = SpicaSDK.Runtime.WebSocketClient.Interfaces.IWebSocket;

namespace SpicaSDK.Services.WebSocketClient
{
    public class WebSocketConnection : IWebSocketConnection
    {
        protected IWebSocket socket;
        protected CompositeDisposable subscriptions;

        private bool disconnected;

        public WebSocketConnection(IWebSocket socket)
        {
            subscriptions = new CompositeDisposable(16);

            this.socket = socket;

            socket.ObserveOpen.First()
                .Subscribe(unit => SpicaLogger.Instance.Log($"[ {nameof(WebSocketClient)} ] Connection established"));

            socket.ObserveError.Subscribe(s => SpicaLogger.Instance.LogWarning(nameof(WebSocketConnection),
                $"[ {nameof(WebSocketClient)} ] Connection Error:\n{s}"));

            socket.ObserveClose.Subscribe(code =>
            {
                SpicaLogger.Instance.Log($"[ {nameof(WebSocketClient)} ] Connection closed with code: {code}");

                if (code != WebSocketCloseCode.Normal)
                    Dispose();
            });
        }

        public IObservable<WebSocketCloseCode> ObserveClose => socket.ObserveClose;
        public IObservable<WebSocketState> ObserveState => socket.ObserveState;

        public void ReconnectWhen(Predicate<WebSocketCloseCode> condition)
        {
            socket.Reconnect(condition);
        }

        public async UniTask DisconnectAsync()
        {
            disconnected = true;
            Dispose();
            await socket.Close();
        }

        void Dispose()
        {
            subscriptions.Clear();
        }

        public async UniTask SendMessageAsync(string message)
        {
            if (disconnected)
                return;

            await socket.SendText(message);
        }
    }
}