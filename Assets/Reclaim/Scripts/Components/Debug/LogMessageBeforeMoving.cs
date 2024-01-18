using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMessageBeforeMoving : EntityComponentBehavior
{
    //public override void WakeUp(IComponent data = null)
    //{
    //    RegisteredEvents.Add(GameEventId.BeforeMoving, BeforeMoving);
    //}

    public void BeforeMoving(GameEvent gameEvent)
    {
        RecLog.Log("something moved");
    }
}
