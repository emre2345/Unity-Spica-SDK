using System;
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
            this.socket = socket;
            
            ObserveOpen.First()
                .Subscribe(unit => Debug.Log($"[ {nameof(WebSocketClient)} ] Connection established"));

            ObserveError.Subscribe(s => Debug.LogError($"[ {nameof(WebSocketClient)} ] Connection Error:\n{s}"));
        }

        public IDisposable Subscribe(IObserver<Message> observer)
        {
            update = Observable.EveryUpdate().Subscribe(l => this.socket.DispatchMessageQueue());

            return ObserveState.Where(state => state == WebSocketState.Open).Select(state =>
            {
                return ObserveMessage.Select(s => JsonConvert.DeserializeObject<Message>(s));
            }).Switch().Subscribe(observer).AddTo(subscriptions);
        }


        public void Disconnect()
        {
            update.Dispose();
            socket.Close();
            subscriptions.Clear();
        }

        public void SendMessage(string message)
        {
            socket.SendText(message);
        }
    }
}