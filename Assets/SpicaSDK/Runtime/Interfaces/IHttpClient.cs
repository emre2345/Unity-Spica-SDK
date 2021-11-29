using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace SpicaSDK.Interfaces
{
    public interface IHttpClient
    {
        void AddDefaultHeader(string key, string value);
        
        UniTask<Response> GetAsync(Request request);

        UniTask<Response> PostAsync(Request request);

        UniTask<Response> PatchAsync(Request request);

        UniTask<Response> DeleteAsync(Request request);

        UniTask<Response> PutAsync(Request request);
    }
}