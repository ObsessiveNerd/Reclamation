using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMessageBeforeMoving : EntityComponent
{
    public override void WakeUp(IComponentData data = null)
    {
        RegisteredEvents.Add(GameEventId.BeforeMoving, BeforeMoving);
    }

    public void BeforeMoving(GameEvent gameEvent)
    {
        RecLog.Log("something moved");
    }
}
