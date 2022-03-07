using System;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using SpicaSDK.Services.WebSocketClient.Extensions;
using UniRx;
using IWebSocket = SpicaSDK.Runtime.WebSocketClient.Interfaces.IWebSocket;

namespace SpicaSDK.Services.WebSocketClient
{
    public class WebSocketConnection : IWebSocketConnection
    {
        protected IWebSocket socket;
        // protected CompositeDisposable subscriptions;

        protected bool disconnected;
        private Predicate<WebSocketCloseCode> reconnectionCondition;

        public WebSocketConnection(IWebSocket socket)
        {
            // subscriptions = new CompositeDisposable(16);

            this.socket = socket;

            // socket.ObserveClose.Where(code => reconnectionCondition?.Invoke(code) ?? false)
                // .Where(code => code != WebSocketCloseCode.Normal).Subscribe(code => Dispose());
        }

        public void ReconnectWhen(Predicate<WebSocketCloseCode> condition)
        {
            reconnectionCondition = condition;
            socket.Reconnect(condition);
        }

        public virtual async UniTask DisconnectAsync()
        {
            disconnected = true;
            // Dispose();
            await socket.Close();
        }

        // protected void Dispose()
        // {
        //     subscriptions.Clear();
        // }

        public async UniTask SendMessageAsync(string message)
        {
            if (disconnected)
                return;

            await socket.SendText(message);
        }
    }
}