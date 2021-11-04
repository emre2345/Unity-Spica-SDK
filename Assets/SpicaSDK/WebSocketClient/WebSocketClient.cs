using System;
using SpicaSDK.Interfaces;

namespace SpicaSDK.Services.WebSocketClient
{
    public class WebSocketClient : IWebSocketClient
    {
        public IObservable<Message> Connect(string url)
        {
            throw new System.NotImplementedException();
        }

        public void Disconnect()
        {
            throw new System.NotImplementedException();
        }

        public void SendMessage(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}