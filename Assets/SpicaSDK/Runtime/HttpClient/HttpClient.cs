using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Cysharp.Threading.Tasks;
using SpicaSDK.Interfaces;
using SpicaSDK.Runtime.Utils;
using SpicaSDK.Services.Exceptions;
using UnityEngine;
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
            if (!defaultHeaders.ContainsKey(key))
                defaultHeaders.Add(key, string.Empty);

            defaultHeaders[key] = value;
        }


        public UniTask<Response> PostAsync(Request request)
        {
            return CreateAndSendRequest(() =>
            {
                SpicaLogger.Instance.Log(
                    $"[{nameof(HttpClient)}] Post request: {request.Url}\nBody:\n{request.Payload}");
                var req = new UnityWebRequest(request.Url, "POST");
                var uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(request.Payload));
                uploadHandler.contentType = "application/json";
                req.uploadHandler = uploadHandler;
                req.downloadHandler = new DownloadHandlerBuffer();
                SetHeaders(req, request.Headers);
                req.SetRequestHeader("Content-Type", "application/json");
                return req;
            });
        }

        public UniTask<Response> PatchAsync(Request request)
        {
            return CreateAndSendRequest(() =>
            {
                var req = new UnityWebRequest(request.Url, "patch");
                var uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(request.Payload));
                uploadHandler.contentType = "application/json";
                req.uploadHandler = uploadHandler;
                req.downloadHandler = new DownloadHandlerBuffer();
                SetHeaders(req, request.Headers);
                req.SetRequestHeader("Content-Type", "application/json");
                return req;
            });
        }

        public UniTask<Response> DeleteAsync(Request request)
        {
            return CreateAndSendRequest(() =>
            {
                var req = new UnityWebRequest(request.Url, "delete");
                req.downloadHandler = new DownloadHandlerBuffer();
                SetHeaders(req, request.Headers);
                return req;
            });
        }

        public UniTask<Response> PutAsync(Request request)
        {
            return CreateAndSendRequest(() =>
            {
                var req = new UnityWebRequest(request.Url, "put");
                var uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(request.Payload));
                uploadHandler.contentType = "application/json";
                req.uploadHandler = uploadHandler;
                req.downloadHandler = new DownloadHandlerBuffer();
                SetHeaders(req, request.Headers);
                req.SetRequestHeader("Content-Type", "application/json");
                return req;
            });
        }

        public UniTask<Response> GetAsync(Request request)
        {
            return CreateAndSendRequest(() =>
            {
                var url = request.Url;
                if (!string.IsNullOrEmpty(request.Payload))
                    url += $"?{request.Payload}";

                SpicaLogger.Instance.Log($"[{nameof(HttpClient)}] Get request: {url}");
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

            SetHeaders(req, defaultHeaders);

            try
            {
                var operation = await req.SendWebRequest();

                SpicaLogger.Instance.Log(
                    $"[{nameof(HttpClient)}] Received Response:\nUrl: {req.url}\nResponse Code: {operation.responseCode}\nResponse:\n{operation.downloadHandler.text}");

                var response = new Response((HttpStatusCode)operation.responseCode, operation.downloadHandler.text,
                    operation.GetResponseHeaders());

                req.Dispose();
                return response;
            }
            catch (UnityWebRequestException e)
            {
                SpicaLogger.Instance.Log(nameof(HttpClient),
                    $"[{nameof(HttpClient)} - RequestException] Received Response:\nUrl: {req.url}\nResponse Code: {e.ResponseCode}\nResponse Text: {e.Text}");
                req.Dispose();
                throw new SpicaWebRequestException(e);
            }
        }
    }
}