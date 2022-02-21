using Cysharp.Threading.Tasks;

namespace SpicaSDK.Runtime.WebSocketClient.Interfaces
{
    public interface IReconnectable : IConnectionStateOwner
    {
        UniTask Connect();
    }
}