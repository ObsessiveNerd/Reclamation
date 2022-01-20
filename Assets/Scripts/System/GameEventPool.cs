using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEventPool
{
    public static Stack<GameEvent> m_Pool = new Stack<GameEvent>();
    public static List<GameEvent> m_InUse = new List<GameEvent>();

    public static void Initialize()
    {
        //for(int i = 0; i < 100; i++)
        //    m_Pool.Push(new GameEvent(""));
    }

    public static GameEvent Get(string id)
    {
        if (m_Pool.Count > 0)
        {
            var ge = m_Pool.Pop();
                ge.SetId(id);
                m_InUse.Add(ge);
                return ge;
        }

        var ge2 = new GameEvent(id);
        return ge2;
    }

    public static void Release(GameEvent obj)
    {
        m_InUse.Remove(obj);
        obj.Clean();
        //if(m_Pool.Contains(obj))
        //    Debug.LogError("GameEvent is already in the Pool, cannot release a GameEvent twice.");
        //else
            m_Pool.Push(obj);
    }
}