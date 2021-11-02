using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace SpicaSDK.Interfaces
{
    public interface IHttpClient
    {
        UniTask<Response> Get(Request request);

        UniTask<Response> Post(Request request);
    }
}