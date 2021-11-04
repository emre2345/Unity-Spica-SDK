using System;
using SpicaSDK.Services.WebSocketClient;

namespace SpicaSDK.Interfaces
{
    public interface IWebSocketClient
    {
        IObservable<Message> Connect(string url);

        void Disconnect();

        void SendMessage(string message);
    }
}