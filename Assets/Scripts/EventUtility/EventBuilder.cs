using System.Collections;
using System.Collections.Generic;

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
        GameEvent ge = new GameEvent(m_Id, m_Values);
        return ge;
    }
}