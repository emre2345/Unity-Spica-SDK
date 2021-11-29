using System;
using Newtonsoft.Json;
using SpicaSDK.Interfaces;
using SpicaSDK.Services.WebSocketClient;
using UniRx;
using UnityEngine;

namespace SpicaSDK.Services.Models
{
    public class DocumentChange<T> : IObservable<T> where T : class
    {
        private IWebSocketConnection connection;

        public DocumentChange(IWebSocketConnection connection)
        {
            this.connection = connection;
        }

        private bool IsKindObservable(DataChangeType changeType)
        {
            return changeType != DataChangeType.Response || changeType != DataChangeType.EndOfInitial;
        }

        bool receivedInitial;

        private bool CheckIfInitialReceived(ServerMessage message)
        {
            if (message.Kind == DataChangeType.Initial)
            {
                receivedInitial = true;
                return true;
            }

            if (message.Kind == DataChangeType.EndOfInitial)
            {
                return receivedInitial;
            }

            return true;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return connection.TakeWhile(CheckIfInitialReceived)
                .Where(message => IsKindObservable(message.Kind) && message.Document != null)
                .TakeWhile(message => message.Kind != DataChangeType.Delete)
                .Select(message => message.Document.ToObject<T>()).Finally(() => connection.DisconnectAsync())
                .Subscribe(observer);
        }
    }
}