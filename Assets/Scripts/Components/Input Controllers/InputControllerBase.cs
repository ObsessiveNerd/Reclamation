using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputControllerBase : Component
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.UpdateEntity);
    }
}
