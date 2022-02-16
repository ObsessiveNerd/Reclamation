using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEventPool
{
    static Stack<GameEvent> m_Pool = new Stack<GameEvent>();
    static List<GameEvent> m_InUse = new List<GameEvent>();

    public static bool GameEventsInUse
    {
        get
        {
            return m_InUse.Count > 0;
        }
    }

    public static List<GameEvent> InUse => m_InUse;

    public static GameEvent Get(string id)
    {
        if (m_Pool.Count > 0)
        {
            var ge = m_Pool.Pop();
            if (string.IsNullOrEmpty(ge.ID))
            {
                ge.SetId(id);
                m_InUse.Add(ge);
                return ge;
            }
        }

        var ge2 = new GameEvent(id);
        m_InUse.Add(ge2);
        return ge2;
    }

    public static void Release(GameEvent obj)
    {
        m_InUse.Remove(obj);
        obj.Clean();
        //if(m_Pool.Contains(obj))
        //    Debug.LogError("GameEvent is already in the Pool, cannot release a GameEvent twice.");
        //else
        if (obj.Paramters.Count > 0)
                throw new System.Exception();
            m_Pool.Push(obj);
    }
}