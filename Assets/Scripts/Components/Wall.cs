using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Component
{
    public Wall()
    {
        RegisteredEvents.Add(GameEventId.BeforeMoving);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        RecLog.Log("OUCH!  You bumped into a wall.");
        gameEvent.Paramters[EventParameters.RequiredEnergy] = 0f;
        gameEvent.Paramters[EventParameters.InputDirection] = MoveDirection.None;
    }
}

public class DTO_Wall : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Wall();
    }
}