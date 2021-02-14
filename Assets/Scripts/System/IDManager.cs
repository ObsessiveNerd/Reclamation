using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IDManager
{
    private static int m_ID = 0;

    public static int GetNewID()
    {
        return ++m_ID;
    }
}
