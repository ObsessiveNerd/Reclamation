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
        gameEvent.Paramters[EventParameter.BlocksMovement] = true;
        gameEvent.Paramters[EventParameter.Weight] = Pathfinder.ImpassableWeight;
    }
}