using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public interface IPoolableObject
//{

//}

public class ObjectPool
{
    static Stack<GameEvent> m_Pool = new Stack<GameEvent>();
    static List<GameEvent> m_InUse = new List<GameEvent>();

    public static GameEvent Get(string id, params KeyValuePair<string, object>[] parameters)
    {
        if (m_Pool.Count == 0)
        {
            var newGe = new GameEvent();
            newGe.Setup(id, parameters);
            m_InUse.Add(newGe);
            return newGe;
        }
        var ge = m_Pool.Pop();
        ge.Setup(id, parameters);
        m_InUse.Add(ge);
        return ge;
    }

    public static GameEvent Get(string id, Dictionary<string, object> values)
    {
        if (m_Pool.Count == 0)
        {
            var newGe = new GameEvent();
            newGe.Setup(id, values);
            m_InUse.Add(newGe);
            return newGe;
        }
        var ge = m_Pool.Pop();
        ge.Setup(id, values);
        m_InUse.Add(ge);
        return ge;
    }

    public static void ReturnAll()
    {
        foreach (var ge in m_InUse)
            Return(ge);
        m_InUse.Clear();
    }

    public static void Return(GameEvent ge)
    {
        ge.Clean();
        m_Pool.Push(ge);
    }
}
