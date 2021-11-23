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


        private IObservable<Unit> ObserveOpen =>
            Observable.FromEvent<WebSocketOpenEventHandler, Unit>(action => () => action.Invoke(Unit.Default),
                action => socket.OnOpen += action, action => socket.OnOpen -= action);

        private IObservable<string> ObserveError => Observable.FromEvent<WebSocketErrorEventHandler, string>(
            action => (s) => action.Invoke(s),
            action => socket.OnError += action, action => socket.OnError -= action);

        private IObservable<string> ObserveMessage =>
            Observable.FromEvent<WebSocketMessageEventHandler, string>(action =>
                    (data => action.Invoke(System.Text.Encoding.UTF8.GetString(data))),
                handler => socket.OnMessage += handler, handler => socket.OnMessage -= handler);

        private IObservable<WebSocketState> ObserveState =>
            socket.ObserveEveryValueChanged(webSocket => webSocket.State);

        public WebSocketConnection(WebSocket socket)
        {
            subscriptions = new CompositeDisposable(16);

            this.socket = socket;

            ObserveOpen.First()
                .Subscribe(unit => Debug.Log($"[ {nameof(WebSocketClient)} ] Connection established"));

            ObserveError.Subscribe(s => Debug.LogWarning($"[ {nameof(WebSocketClient)} ] Connection Error:\n{s}"));
        }

        public IDisposable Subscribe(IObserver<ServerMessage> observer)
        {
            update = Observable.EveryUpdate().Subscribe(l => this.socket.DispatchMessageQueue());

            return ObserveState.Where(state => state == WebSocketState.Open).Select(state =>
            {
                return ObserveMessage.Do(s => Debug.Log($"[ {nameof(WebSocketClient)} ] - Message Received: {s}"))
                    .Select(s => JsonConvert.DeserializeObject<ServerMessage>(s));
            }).Switch().Subscribe(observer).AddTo(subscriptions);
        }


        public void Disconnect()
        {
            update.Dispose();
            subscriptions.Clear();
            socket.Close();
        }

        public void SendMessage(string message)
        {
            socket.SendText(message);
        }
    }
}