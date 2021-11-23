using System;
using SpicaSDK.Services.WebSocketClient;

namespace SpicaSDK.Interfaces
{
    public interface IWebSocketClient
    {
        IWebSocketConnection Connect(string url);
    }

    public interface IWebSocketConnection : IObservable<ServerMessage>
    {
        void Disconnect();

        void SendMessage(string message);
    }
}