using System;
using NativeWebSocket;
using SpicaSDK.Runtime.Services.Bucket.Realtime;
using SpicaSDK.Services.WebSocketClient;
using SpicaSDK.Runtime.WebSocketClient.Interfaces;
using UniRx;

namespace SpicaSDK.Services.Models
{
    public class DocumentChange<T> : IConnectionStateOwner, IObservable<T> where T : class
    {
        private IBucketRealtimeConnection connection;

        public DocumentChange(IBucketRealtimeConnection connection)
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

        public IObservable<WebSocketState> ObserveState => connection.ObserveState;
        public IObservable<WebSocketCloseCode> ObserveConnectionClose => connection.ObserveConnectionClose;
    }
}