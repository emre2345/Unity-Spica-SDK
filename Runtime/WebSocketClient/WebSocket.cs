using System;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Services.Exceptions;
using UniRx;
using IWebSocket = SpicaSDK.Runtime.WebSocketClient.Interfaces.IWebSocket;

namespace SpicaSDK.Services.WebSocketClient
{
    public class WebSocket : IWebSocket
    {
        // private NativeWebSocket.WebSocket socket;
        private string url;
        private NativeWebSocket.WebSocket socket;
        private IDisposable update;
        private ReactiveProperty<WebSocketState> state;

        private Subject<string> message;
        private Subject<Unit> open;
        private Subject<string> error;
        private Subject<WebSocketCloseCode> close;

        public WebSocket(string url)
        {
            this.url = url;

            message = new Subject<string>();
            open = new Subject<Unit>();
            error = new Subject<string>();
            close = new Subject<WebSocketCloseCode>();
        }

        public IObservable<Unit> ObserveOpen => open;
        public IObservable<string> ObserveError => error;
        public IObservable<string> ObserveMessage => message;
        public IObservable<WebSocketCloseCode> ObserveClose => close;

        public IObservable<WebSocketState> ObserveState => state;

        public WebSocketState State => state.Value;


        public UniTask Connect()
        {
            CreateSocket();
            StartMessageDispatch();

            return socket.Connect().AsUniTask();
        }

        void CreateSocket()
        {
            socket = new NativeWebSocket.WebSocket(url);
            state = new ReactiveProperty<WebSocketState>(WebSocketState.Closed);

            CreateObservables();

            ObserveOpen.First()
                .Subscribe(unit => SpicaLogger.Instance.Log($"[ {nameof(WebSocket)} ] Connection established"));

            ObserveError.Subscribe(s => SpicaLogger.Instance.LogWarning(nameof(WebSocket),
                $"[ {nameof(WebSocket)} ] Connection Error:\n{s}"));

            ObserveClose.Subscribe(code =>
            {
                SpicaLogger.Instance.Log($"[ {nameof(WebSocketClient)} ] Connection closed with code: {code}");
            });
        }

        void CreateObservables()
        {
            Observable.FromEvent<WebSocketOpenEventHandler, Unit>(
                action => () => action.Invoke(Unit.Default),
                action => socket.OnOpen += action, action => socket.OnOpen -= action).Subscribe(open);

            Observable.FromEvent<WebSocketErrorEventHandler, string>(
                action => (s) => action.Invoke(s),
                action => socket.OnError += action, action => socket.OnError -= action).Subscribe(error);

            Observable.FromEvent<WebSocketMessageEventHandler, string>(action =>
                    (data => action.Invoke(System.Text.Encoding.UTF8.GetString(data))),
                handler => socket.OnMessage += handler, handler => socket.OnMessage -= handler).Do(s =>
                SpicaLogger.Instance.Log($"[ {nameof(WebSocketClient)} ] - Message Received: {s}")).Subscribe(message);

            Observable.FromEvent<WebSocketCloseEventHandler, WebSocketCloseCode>(action => action.Invoke,
                action => socket.OnClose += action,
                action => socket.OnClose -= action).Subscribe(close);

            socket.ObserveEveryValueChanged(webSocket => webSocket.State)
                .Subscribe(socketState => state.Value = socketState);
        }

        void StartMessageDispatch()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            update?.Dispose();
            update = Observable.EveryUpdate().Subscribe(l => socket.DispatchMessageQueue());
#endif
        }

        public UniTask Close()
        {
            update?.Dispose();

            return socket.Close().AsUniTask();
        }

        public async UniTask SendText(string message)
        {
            try
            {
                await socket.SendText(message);
            }
            catch (ObjectDisposedException e)
            {
                throw new SpicaServerException(e.Message, e);
            }
        }
    }
}