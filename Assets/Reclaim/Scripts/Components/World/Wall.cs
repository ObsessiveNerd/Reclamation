using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : EntityComponent
{
    public void Start()
    {
        RegisteredEvents.Add(GameEventId.BeforeMoving, BeforeMoving);
        RegisteredEvents.Add(GameEventId.PathfindingData, PathfindingData);
    }

    void BeforeMoving(GameEvent gameEvent)
    {
        RecLog.Log("OUCH!  You bumped into a wall.");
        gameEvent.Paramters[EventParameter.RequiredEnergy] = 0f;
        gameEvent.Paramters[EventParameter.InputDirection] = MoveDirection.None;
    }

    void PathfindingData(GameEvent gameEvent)
    {
        var flags = gameEvent.GetValue<MovementBlockFlag>(EventParameter.BlocksMovementFlags);
        flags |= MovementBlockFlag.All;
        gameEvent.SetValue<MovementBlockFlag>(EventParameter.BlocksMovementFlags, flags);
        gameEvent.Paramters[EventParameter.Weight] = Pathfinder.ImpassableWeight;
    }
}