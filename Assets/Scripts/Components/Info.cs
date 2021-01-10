using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info : Component
{
    string m_Info;
    public override int Priority { get { return 6; } }

    public Info(IEntity self, string info)
    {
        Init(self);
        m_Info = info;

        RegisteredEvents.Add(GameEventId.ShowInfo);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        RecLog.Log(m_Info);
    }
}
