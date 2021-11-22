using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Cysharp.Threading.Tasks;
using SpicaSDK.Interfaces;
using UnityEngine.Networking;

namespace SpicaSDK
{
    public class HttpClient : IHttpClient
    {
        private Dictionary<string, string> defaultHeaders;

        public HttpClient()
        {
            defaultHeaders = new Dictionary<string, string>(16);
        }

        public void AddDefaultHeader(string key, string value)
        {
            if(!defaultHeaders.ContainsKey(key))
                defaultHeaders.Add(key, string.Empty);

            defaultHeaders[key] = value;
        }


        UniTask<Response> IHttpClient.Post(Request request)
        {
            return CreateAndSendRequest(() =>
            {
                var req = new UnityWebRequest(request.Url, "POST");
                var uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(request.Payload));
                uploadHandler.contentType = "application/json";
                req.uploadHandler = uploadHandler;
                req.downloadHandler = new DownloadHandlerBuffer();
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
                var url = request.Url;
                if (!string.IsNullOrEmpty(request.Payload))
                    url += request.Payload;
                
                var req = UnityWebRequest.Get(url);
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
            req.SetRequestHeader("Content-Type", "application/json");
            
            SetHeaders(req, defaultHeaders);
            
            var operation = await req.SendWebRequest();
            return new Response((HttpStatusCode)operation.responseCode, operation.downloadHandler.text);
        }
    }
}