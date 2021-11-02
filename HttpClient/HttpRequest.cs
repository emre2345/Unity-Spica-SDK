using System;
using System.Net;
using Cysharp.Threading.Tasks;
using SpicaSDK.Interfaces;
using UnityEngine.Networking;

namespace SpicaSDK
{
    public class HttpRequest : IHttpClient
    {
        UniTask<Response> IHttpClient.Post(Request request)
        {
            return CreateRequest(() => UnityWebRequest.Post(request.Url, request.Payload));
        }

        UniTask<Response> IHttpClient.Get(Request request)
        {
            return CreateRequest(() => UnityWebRequest.Get(request.Url));
        }

        private async UniTask<Response> CreateRequest(Func<UnityWebRequest> factory)
        {
            var req = factory();
            var operation = await req.SendWebRequest();
            return new Response((HttpStatusCode)operation.responseCode, operation.downloadHandler.text);
        }
    }
}