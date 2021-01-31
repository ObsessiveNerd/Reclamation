using System;
using UnityEngine;

public static class RecLog
{
    public static Action<string> MessageLogged = (message) => Debug.Log(message);

    public static void Log(string log)
    {
        MessageLogged(log);
    }

    public static void Log(object log)
    {
        MessageLogged(log.ToString());
    }
}
