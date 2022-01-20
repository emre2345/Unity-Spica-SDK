using System;
using Cysharp.Threading.Tasks;
using SpicaSDK.Interfaces;
using SpicaSDK.Runtime.Utils;
using UnityEngine;

namespace Plugins.SpicaSDK.Runtime.Services.Function.Firehose
{
    public class Firehose
    {
        private ISpicaServer spicaServer;
        private IWebSocketClient webSocketClient;

        public Firehose(ISpicaServer spicaServer, IWebSocketClient webSocketClient)
        {
            this.spicaServer = spicaServer;
            this.webSocketClient = webSocketClient;
        }

        public async UniTask<FirehoseConnection<T>> Connect<T>(QueryParams queryParams)
        {
            var connection =
                await webSocketClient.ConnectAsync($"{spicaServer.FirehoseUrl}?{queryParams.GetString()}",
                    socket => new FirehoseConnection<T>(socket));
            return connection as FirehoseConnection<T>;
        }
    }
}