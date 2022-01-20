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
        protected CompositeDisposable subscriptions;

        private bool disconnected;

        protected IObservable<Unit> observeOpen;
        protected IObservable<string> observeError;
        protected IObservable<string> observeMessage;
        protected IObservable<WebSocketCloseCode> observeClose;
        protected IObservable<WebSocketState> observeState;

        public WebSocketConnection(WebSocket socket)
        {
            subscriptions = new CompositeDisposable(16);

            this.socket = socket;

            CreateObservables();

            observeOpen.First()
                .Subscribe(unit => Debug.Log($"[ {nameof(WebSocketClient)} ] Connection established"));

            observeError.Subscribe(s => Debug.LogWarning($"[ {nameof(WebSocketClient)} ] Connection Error:\n{s}"));

            observeClose.Subscribe(code =>
                Debug.Log($"[ {nameof(WebSocketClient)} ] Connection closed with code: {code}"));

            StartMessageDispatch();
        }

        void StartMessageDispatch()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            update = Observable.EveryUpdate().Subscribe(l => socket.DispatchMessageQueue());
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

            observeClose = Observable.FromEvent<WebSocketCloseEventHandler, WebSocketCloseCode>(action => action.Invoke,
                action => socket.OnClose += action,
                action => socket.OnClose -= action);

            observeState = socket.ObserveEveryValueChanged(webSocket => webSocket.State).Share();
        }

        // public IDisposable Subscribe(IObserver<ServerMessage> observer)
        // {
        //     return observeMessage
        //         .Select(s => JsonConvert.DeserializeObject<ServerMessage>(s)).Subscribe(observer).AddTo(subscriptions);
        // }

        public UniTask Connected()
        {
            return UniTask.WaitUntil(() => socket.State == WebSocketState.Open);
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