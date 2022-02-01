using System;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using SpicaSDK.Runtime.Services.Function.Firehose;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using SpicaSDK.Services.WebSocketClient;
using UniRx;
using UniRx.Diagnostics;
using UnityEngine;

namespace Plugins.SpicaSDK.Runtime.Services.Function.Firehose
{
    public class FirehoseConnection<T> : WebSocketConnection, IFirehoseConnection<T>
    {
        private IObservable<T> sharedMessage;

        public FirehoseConnection(WebSocket socket) : base(socket)
        {
            sharedMessage = observeMessage
                .Select(s => JsonConvert.DeserializeObject<T>(s))
                .Do(s => Debug.Log($"[{nameof(FirehoseConnection<T>)}] Received message: {s}")).Share();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return sharedMessage.Subscribe(observer)
                .AddTo(subscriptions);
        }

        public async UniTask SendMessage(FirehoseMessage message)
        {
            Debug.Log($"[{nameof(FirehoseConnection<T>)}] Sending message: {message}");
            await SendMessageAsync(message.ToString());
        }
    }
}