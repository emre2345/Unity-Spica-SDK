using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace SpicaSDK.Interfaces
{
    public interface IHttpClient
    {
        void AddDefaultHeader(string key, string value);
        
        UniTask<Response> Get(Request request);

        UniTask<Response> Post(Request request);

        UniTask<Response> Patch(Request request);

        UniTask<Response> Delete(Request request);

        UniTask<Response> Put(Request request);
    }
}