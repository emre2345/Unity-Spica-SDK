using System;
using System.Collections.Generic;
using System.Net;
using Cysharp.Threading.Tasks;
using SpicaSDK.Interfaces;
using UnityEngine.Networking;

namespace SpicaSDK
{
    public class HttpClient : IHttpClient
    {
        UniTask<Response> IHttpClient.Post(Request request)
        {
            return CreateAndSendRequest(() =>
            {
                var req = UnityWebRequest.Post(request.Url, request.Payload);
                SetHeaders(req, request.Headers);
                return req;
            });
        }

        public UniTask<Response> Patch(Request request)
        {
            return CreateAndSendRequest(() =>
            {
                var req = new UnityWebRequest(request.Url, "patch");
                SetHeaders(req, request.Headers);
                return req;
            });
        }

        public UniTask<Response> Delete(Request request)
        {
            return CreateAndSendRequest(() =>
            {
                var req = new UnityWebRequest(request.Url, "delete");
                SetHeaders(req, request.Headers);
                return req;
            });
        }

        public UniTask<Response> Put(Request request)
        {
            return CreateAndSendRequest(() =>
            {
                var req = new UnityWebRequest(request.Url, "put");
                SetHeaders(req, request.Headers);
                return req;
            });
        }

        UniTask<Response> IHttpClient.Get(Request request)
        {
            return CreateAndSendRequest(() =>
            {
                var req = UnityWebRequest.Get($"{request.Url}&{request.Payload}");
                SetHeaders(req, request.Headers);
                return req;
            });
        }

        private void SetHeaders(UnityWebRequest request, Dictionary<string, string> headers)
        {
            foreach (var requestHeader in headers)
            {
                request.SetRequestHeader(requestHeader.Key, requestHeader.Value);
            }
        }

        private async UniTask<Response> CreateAndSendRequest(Func<UnityWebRequest> factory)
        {
            var req = factory();
            var operation = await req.SendWebRequest();
            return new Response((HttpStatusCode)operation.responseCode, operation.downloadHandler.text);
        }
    }
}