using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMessageBeforeMoving : Component
{
    public LogMessageBeforeMoving()
    {
        RegisteredEvents.Add(GameEventId.BeforeMoving);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        RecLog.Log("something moved");
    }
}
