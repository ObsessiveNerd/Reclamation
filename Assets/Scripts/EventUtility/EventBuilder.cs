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

        var pooledObj = m_Pool.Pop();
        pooledObj.Id = id;
        return pooledObj;
    }

    public static void Return(EventBuilder e)
    {
        e.Clean();
        m_Pool.Push(e);
    }
}

public struct EventBuilder
{
    public string Id;
    //private Dictionary<string, object> m_Values;
    private KeyValuePair<string, object>[] m_Values;
    int index;

    public EventBuilder(string eventId)
    {
        Id = eventId;
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
        GameEvent ge = ObjectPool.Get(Id, m_Values); //new GameEvent(m_Id, m_Values);
        EventBuilderPool.Return(this);
        return ge;
    }

    public void Clean()
    {
        Id = "";
        for (int i = 0; i < 10; i++)
            m_Values[i] = default(KeyValuePair<string, object>);
        index = 0;
    }
}