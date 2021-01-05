using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Component
{
    public Wall(IEntity self)
    {
        Init(self);
        RegisteredEvents.Add(GameEventId.BeforeMoving);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        Debug.Log("OUCH!  You bumped into a wall.");
        gameEvent.Paramters[EventParameters.RequiredEnergy] = 0f;
        gameEvent.Paramters[EventParameters.InputDirection] = MoveDirection.None;
    }
}
