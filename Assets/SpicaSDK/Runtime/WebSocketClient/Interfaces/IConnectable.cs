using Cysharp.Threading.Tasks;

namespace SpicaSDK.Runtime.WebSocketClient.Interfaces
{
    public interface IConnectable : IConnectionStateOwner
    {
        UniTask Connect();
    }
}