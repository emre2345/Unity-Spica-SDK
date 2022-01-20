using System;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using SpicaSDK.Services.WebSocketClient;
using UniRx;
using UniRx.Diagnostics;
using UnityEngine;

namespace Plugins.SpicaSDK.Runtime.Services.Function.Firehose
{
    public class FirehoseConnection<T> : WebSocketConnection, IFirehoseConnection<T>
    {
        public FirehoseConnection(WebSocket socket) : base(socket)
        {
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return observeMessage
                .Select(s => JsonConvert.DeserializeObject<T>(s))
                .Do(s => Debug.Log($"[{nameof(FirehoseConnection<T>)}] Received message: {s}")).Subscribe(observer)
                .AddTo(subscriptions);
        }

        public async UniTask SendMessage(FirehoseMessage message)
        {
            Debug.Log($"[{nameof(FirehoseConnection<T>)}] Sending message: {message}");
            await SendMessageAsync(message.ToString());
        }
    }
}