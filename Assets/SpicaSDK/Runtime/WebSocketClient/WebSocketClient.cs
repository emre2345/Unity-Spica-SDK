using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using UniRx;
using UnityEngine;
using IWebSocket = SpicaSDK.Runtime.WebSocketClient.Interfaces.IWebSocket;

namespace SpicaSDK.Services.WebSocketClient
{
    public class WebSocketClient : IWebSocketClient
    {
        private List<IWebSocketConnection> allConnections;

        public WebSocketClient()
        {
            allConnections = new List<IWebSocketConnection>();
        }

        ~WebSocketClient()
        {
            Dispose();
        }

        public async UniTask<IWebSocketConnection> ConnectAsync(string url,
            Func<IWebSocket, IWebSocketConnection> connectionFactory)
        {
            SpicaLogger.Instance.Log($"WS Connecting to: {url}");
            var socket = new WebSocket(url);
            socket.Connect();
            await socket.ObserveEveryValueChanged(webSocket => webSocket.State)
                .First(state => state == WebSocketState.Open).ToUniTask();

            var connection = connectionFactory(socket); //new BucketRealtimeConnection(socket);
            allConnections.Add(connection);
            return connection;
        }

        public void Dispose()
        {
            allConnections.ForEach(connection => connection.DisconnectAsync());
        }
    }
}