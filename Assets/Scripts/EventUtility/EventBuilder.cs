using System.Collections;
using System.Collections.Generic;

public class EventBuilder
{
    private string m_Id;
    private Dictionary<string, object> m_Values = new Dictionary<string, object>();

    public EventBuilder(string eventId)
    {
        m_Id = eventId;
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