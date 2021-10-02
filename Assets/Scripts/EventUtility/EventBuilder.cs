using System.Collections;
using System.Collections.Generic;

public struct EventBuilder
{
    private string m_Id;
    private Dictionary<string, object> m_Values;

    public EventBuilder(string eventId)
    {
        m_Id = eventId;
        m_Values = new Dictionary<string, object>();
    }

    public EventBuilder With(string key, object value)
    {
        m_Values.Add(key, value);
        return this;
    }

    public GameEvent CreateEvent()
    {
        GameEvent ge = new GameEvent(m_Id, m_Values);
        return ge;
    }
}