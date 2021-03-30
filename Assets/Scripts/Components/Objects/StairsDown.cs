using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsDown : Component
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.Interact);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.Interact)
        {

        }
    }
}
