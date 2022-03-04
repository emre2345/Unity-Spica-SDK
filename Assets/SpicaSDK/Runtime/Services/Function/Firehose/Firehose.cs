using System;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using SpicaSDK.Interfaces;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using UniRx;
using UnityEngine;
using IWebSocket = SpicaSDK.Runtime.WebSocketClient.Interfaces.IWebSocket;

namespace Plugins.SpicaSDK.Runtime.Services.Function.Firehose
{
    public class Firehose
    {
        private ISpicaServer spicaServer;
        private IWebSocketClient webSocketClient;

        private IWebSocket webSocket;
        private IntReactiveProperty subscriptionCount = new(0);

        public Firehose(ISpicaServer spicaServer, IWebSocketClient webSocketClient)
        {
            this.spicaServer = spicaServer;
            this.webSocketClient = webSocketClient;

            subscriptionCount.Skip(1).Where(i => i == 0).Subscribe(_ =>
            {
                webSocket?.Close();
                webSocket = null;
            });
        }

        public async UniTask<FirehoseConnection<T>> Connect<T>(QueryParams queryParams, string filter = "")
        {
            if (webSocket == null || webSocket.State == WebSocketState.Closed ||
                webSocket.State == WebSocketState.Closing)
                await webSocketClient.ConnectAsync($"{spicaServer.FirehoseUrl}?{queryParams.GetString()}",
                    socket =>
                    {
                        webSocket = socket;
                        return null;
                    });

            var connection = new FirehoseConnection<T>(webSocket, filter);
            connection.DoOnSubscribe(() => subscriptionCount.Value++).DoOnTerminate(() => subscriptionCount.Value--);
            return connection;
        }
    }
}