using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IDManager
{
    public static string GetNewID()
    {
        return Guid.NewGuid().ToString();
    }
}
