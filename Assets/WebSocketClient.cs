using System;
using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using NativeWebSocket;
using UnityEngine;

public class WebSocketClient : MonoBehaviour
{
    public string url = "ws://localhost:4500/api/bucket/6176b0946a95a2002d7dcbf3/data?ApiKey=405opg19kv80szts";

    private WebSocket webSocket;

    void Start()
    {
        Debug.Log("Connecting");
        webSocket = new WebSocket(url);
        
        webSocket.OnOpen += () => { Debug.Log("Connection open!"); };

        webSocket.OnError += (e) => { Debug.Log("Error! " + e); };

        webSocket.OnClose += (e) => { Debug.Log("Connection closed!"); };

        webSocket.OnMessage += (bytes) =>
        {
            // Reading a plain text message
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received OnMessage! (" + bytes.Length + " bytes) " + message);
        };

        // Keep sending messages at every 0.3s
        // InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);
    }

    [Button]
    private void Connect()
    {
        webSocket.Connect();
    }

    [Button]
    private void Disconnect()
    {
        webSocket.Close();
    }

    [Button]
    private void SendMessage(string message)
    {
        webSocket.SendText(message);
    }

    private void Update()
    {
        webSocket.DispatchMessageQueue();
    }

    private void OnDestroy()
    {
        webSocket.Close();
    }
}