using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMessageBeforeMoving : EntityComponent
{
    public void Start()
    {
        RegisteredEvents.Add(GameEventId.BeforeMoving, BeforeMoving);
    }

    public void BeforeMoving(GameEvent gameEvent)
    {
        RecLog.Log("something moved");
    }
}
