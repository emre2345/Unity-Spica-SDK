using System;
using NativeWebSocket;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using UniRx;
using UnityEngine;

namespace SpicaSDK.Services.WebSocketClient
{
    public class WebSocketClient : IWebSocketClient
    {
        
        public IWebSocketConnection Connect(string url)
        {
            var socket = new WebSocket(url);
            socket.Connect();
            return new WebSocketConnection(socket);
        }
    }
}