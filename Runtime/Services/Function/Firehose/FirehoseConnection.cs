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
        private Subject<T> sharedMessage;
        private IDisposable socketSubscription;

        public FirehoseConnection(IWebSocket socket, string filter) : base(socket)
        {
            sharedMessage = new Subject<T>();

            socketSubscription = socket.ObserveMessage
                .Select(s => JsonConvert.DeserializeObject<JObject>(s)).Where(
                    response =>
                    {
                        if (!string.IsNullOrEmpty(filter))
                            return response["name"].Value<string>().Equals(filter);

                        return true;
                    })
                .Select(response => { return JsonConvert.DeserializeObject<T>(response["data"].ToString()); })
                .Do(s => SpicaLogger.Instance.Log($"[{nameof(FirehoseConnection<T>)}] Received message: {s}"))
                .Share().Subscribe(sharedMessage);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return sharedMessage.Subscribe(observer);
        }

        public async UniTask SendMessage(FirehoseMessage message)
        {
            SpicaLogger.Instance.Log($"[{nameof(FirehoseConnection<T>)}] Sending message: {message}");
            await SendMessageAsync(message.ToString());
        }

        public override UniTask DisconnectAsync()
        {
            disconnected = true;
            socketSubscription.Dispose();
            sharedMessage.OnCompleted();
            // Dispose();
            return UniTask.CompletedTask;
        }
    }
}