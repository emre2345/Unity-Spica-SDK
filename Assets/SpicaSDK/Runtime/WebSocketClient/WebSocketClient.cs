using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using UniRx;
using UnityEngine;

namespace SpicaSDK.Services.WebSocketClient
{
    public class WebSocketClient : IWebSocketClient, IDisposable
    {
        private List<WebSocketConnection> allConnections;

        public WebSocketClient()
        {
            allConnections = new List<WebSocketConnection>();
        }

        ~WebSocketClient()
        {
            Dispose();
        }

        public async UniTask<IWebSocketConnection> ConnectAsync(string url)
        {
            Debug.Log($"WS Connecting to: {url}");
            var socket = new WebSocket(url);
            socket.Connect();
            await socket.ObserveEveryValueChanged(webSocket => webSocket.State)
                .First(state => state == WebSocketState.Open).ToUniTask();

            var connection = new WebSocketConnection(socket);
            allConnections.Add(connection);
            return connection;
        }

        public void Dispose()
        {
            allConnections.ForEach(connection => connection.DisconnectAsync());
        }
    }
}