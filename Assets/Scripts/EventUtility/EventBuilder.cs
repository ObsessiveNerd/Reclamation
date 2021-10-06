using System.Collections;
using System.Collections.Generic;

public static class EventBuilderPool
{
    static Stack<EventBuilder> m_Pool = new Stack<EventBuilder>();
    
    public static EventBuilder Get(string id)
    {
        if(m_Pool.Count == 0)
        {
            EventBuilder e = new EventBuilder(id);
            return e;
        }

        return m_Pool.Pop();
    }

    public static void Return(EventBuilder e)
    {
        e.Clean();
        m_Pool.Push(e);
    }
}

public struct EventBuilder
{
    private string m_Id;
    //private Dictionary<string, object> m_Values;
    private KeyValuePair<string, object>[] m_Values;
    int index;

    public EventBuilder(string eventId)
    {
        m_Id = eventId;
        m_Values = new KeyValuePair<string, object>[10];//new Dictionary<string, object>();
        index = 0;
    }

    public EventBuilder With(string key, object value)
    {
        m_Values[index] = new KeyValuePair<string, object>(key, value);
        index++;
        //m_Values.Add(key, value);
        return this;
    }

    public GameEvent CreateEvent()
    {
        GameEvent ge = ObjectPool.Get(m_Id, m_Values); //new GameEvent(m_Id, m_Values);
        return ge;
    }

    public void Clean()
    {
        m_Id = "";
        for (int i = 0; i < 10; i++)
            m_Values[i] = default(KeyValuePair<string, object>);
        index = 0;
    }
}