using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpicaSDK.Runtime.Services.Function.Firehose;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using SpicaSDK.Services.WebSocketClient;
using UniRx;

namespace Plugins.SpicaSDK.Runtime.Services.Function.Firehose
{
    public class FirehoseConnection<T> : WebSocketConnection, IFirehoseConnection<T>
    {
        private IObservable<T> sharedMessage;

        public FirehoseConnection(IWebSocket socket) : base(socket)
        {
            sharedMessage = socket.ObserveMessage
                .Select(s =>
                {
                    JObject response = JsonConvert.DeserializeObject<JObject>(s);
                    return JsonConvert.DeserializeObject<T>(response["data"].ToString());
                })
                .Do(s => SpicaLogger.Instance.Log($"[{nameof(FirehoseConnection<T>)}] Received message: {s}")).Share();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return sharedMessage.Subscribe(observer)
                .AddTo(subscriptions);
        }

        public async UniTask SendMessage(FirehoseMessage message)
        {
            SpicaLogger.Instance.Log($"[{nameof(FirehoseConnection<T>)}] Sending message: {message}");
            await SendMessageAsync(message.ToString());
        }
    }
}