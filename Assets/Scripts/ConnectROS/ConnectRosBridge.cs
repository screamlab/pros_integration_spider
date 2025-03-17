using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
public class ConnectRosBridge : MonoBehaviour
{
    public string rosbridgeServerAddress = "localhost:9090";
    public WebSocket ws;

    // Start is called before the first frame update
    void Start()
    {
        string protocol = "ws://";
        ws = new WebSocket(protocol + rosbridgeServerAddress);
        ws.Connect();
    }

    private void OnDestroy()
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }
    }

    public void PublishFloat32MultiArray(string topic, float[] data)
    {
        string jsonMessage = $@"{{
            ""op"": ""publish"",
            ""topic"": ""{topic}"",
            ""msg"": {{
                ""layout"": {{
                    ""dim"": [{{""label"": ""length"", ""size"": {data.Length}, ""stride"": 1}}],
                    ""data_offset"": 0
                }},
                ""data"": [{string.Join(", ", data)}]
            }}
        }}";

        ws.Send(jsonMessage);
    }
    public void Advertise(string topic, string type)
    {
        string jsonMessage = $@"{{
            ""op"": ""advertise"",
            ""topic"": ""{topic}"",
            ""type"": ""{type}""
        }}";

        ws.Send(jsonMessage);
    }

    public void PublishBool(string topic, bool value)
    {
        string jsonMessage = $@"{{
            ""op"": ""publish"",
            ""topic"": ""{topic}"",
            ""msg"": {{
                ""data"": {value.ToString().ToLower()}
            }}
        }}";

        ws.Send(jsonMessage);
    }

}
