using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    SocketIOComponent socket;

    void Awake()
    {
        socket = GetComponent<SocketIOComponent>();
        socket.SetupSocket();
    }

    public void OnConnect()
    {
        socket.On("enterWorld", OnEnterWorld);
        socket.On("processgameEvent", OnProcessGameEvent);
        socket.Emit("enterWorld", new JSONObject());
    }

    public void OnEnterWorld(SocketIOEvent e)
    {
        Debug.Log("Player entering world");
    }

    public void OnProcessGameEvent(SocketIOEvent e)
    {

    }

    public static void BroadcastGameEvent(string serializedGameEvent)
    {

    }
}
