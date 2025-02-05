using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class Session : MonoBehaviour
{

    [SerializeField] private TMP_Text output;
    private ClientWebSocket ws;
    private CancellationTokenSource cts;

    // Start is called before the first frame update
    async void Start()
    {
        output.text = "Connecting to WebSocket...";
        ws = new ClientWebSocket();
        cts = new CancellationTokenSource();

        try
        {
            await ws.ConnectAsync(new Uri("ws://localhost:8765"), cts.Token);
            output.text = "Connected to WebSocket!";
            ReceiveMessages();
        }
        catch (Exception e)
        {
            output.text = "WebSocket connection failed: " + e.Message;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async void ReceiveMessages()
    {
        var buffer = new byte[1024];

        try
        {
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Debug.Log("Received: " + message);
                    output.text = message;
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("WebSocket error: " + e.Message);
        }
    }

    void OnDestroy()
    {
        // Close the WebSocket connection when the script is destroyed
        if (ws != null)
        {
            cts.Cancel();
            ws.Dispose();
        }
    }
}
