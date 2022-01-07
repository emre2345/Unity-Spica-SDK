using System;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using UniRx;
using UnityEngine;

namespace SpicaSDK.Services.WebSocketClient
{
    public class WebSocketConnection : IWebSocketConnection
    {
        private WebSocket socket;
        private IDisposable update;
        private CompositeDisposable subscriptions;

        private bool disconnected;

        private IObservable<Unit> observeOpen;
        private IObservable<string> observeError;
        private IObservable<string> observeMessage;
        private IObservable<WebSocketState> observeState;

        public WebSocketConnection(WebSocket socket)
        {
            subscriptions = new CompositeDisposable(16);

            this.socket = socket;

            CreateObservables();

            observeOpen.First()
                .Subscribe(unit => Debug.Log($"[ {nameof(WebSocketClient)} ] Connection established"));

            observeError.Subscribe(s => Debug.LogWarning($"[ {nameof(WebSocketClient)} ] Connection Error:\n{s}"));

#if !UNITY_WEBGL || UNITY_EDITOR
            update = Observable.EveryUpdate().Subscribe(l => this.socket.DispatchMessageQueue());
#endif
        }

        void CreateObservables()
        {
            observeOpen = Observable.FromEvent<WebSocketOpenEventHandler, Unit>(
                action => () => action.Invoke(Unit.Default),
                action => socket.OnOpen += action, action => socket.OnOpen -= action).Share();

            observeError = Observable.FromEvent<WebSocketErrorEventHandler, string>(
                action => (s) => action.Invoke(s),
                action => socket.OnError += action, action => socket.OnError -= action).Share();

            observeMessage =
                Observable.FromEvent<WebSocketMessageEventHandler, string>(action =>
                        (data => action.Invoke(System.Text.Encoding.UTF8.GetString(data))),
                    handler => socket.OnMessage += handler, handler => socket.OnMessage -= handler).Do(s =>
                    Debug.Log($"[ {nameof(WebSocketClient)} ] - Message Received: {s}")).Share();

            observeState = socket.ObserveEveryValueChanged(webSocket => webSocket.State).Share();
        }

        public IDisposable Subscribe(IObserver<ServerMessage> observer)
        {
            return observeMessage
                .Select(s => JsonConvert.DeserializeObject<ServerMessage>(s)).Subscribe(observer).AddTo(subscriptions);
        }

        public async UniTask DisconnectAsync()
        {
            disconnected = true;
            update.Dispose();
            subscriptions.Clear();
            await socket.Close();
        }

        public async UniTask SendMessageAsync(string message)
        {
            if (disconnected)
                return;

            await socket.SendText(message);
        }
    }
}