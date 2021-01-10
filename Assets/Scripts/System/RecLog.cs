using UnityEngine;

public static class RecLog
{
    public static void Log(string log)
    {
        Debug.Log(log);
    }

    public static void Log(object log)
    {
        Debug.Log(log);
    }
}
