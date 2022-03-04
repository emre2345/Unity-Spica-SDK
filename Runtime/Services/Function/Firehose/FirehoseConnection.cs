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

        public FirehoseConnection(IWebSocket socket, string filter) : base(socket)
        {
            sharedMessage = socket.ObserveMessage
                .Select(s => JsonConvert.DeserializeObject<JObject>(s)).Where(
                    response =>
                    {
                        if (!string.IsNullOrEmpty(filter))
                            return response["name"].Value<string>().Equals(filter);

                        return true;
                    })
                .Select(response => { return JsonConvert.DeserializeObject<T>(response["data"].ToString()); })
                .Do(s => SpicaLogger.Instance.Log($"[{nameof(FirehoseConnection<T>)}] Received message: {s}"))
                .Share();
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

        public override UniTask DisconnectAsync()
        {
            disconnected = true;
            Dispose();
            return UniTask.CompletedTask;
        }
    }
}