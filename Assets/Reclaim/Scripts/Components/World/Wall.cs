using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : EntityComponent
{
    public Wall()
    {
        RegisteredEvents.Add(GameEventId.BeforeMoving);
        RegisteredEvents.Add(GameEventId.PathfindingData);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.BeforeMoving)
        {
            RecLog.Log("OUCH!  You bumped into a wall.");
            gameEvent.Paramters[EventParameter.RequiredEnergy] = 0f;
            gameEvent.Paramters[EventParameter.InputDirection] = MoveDirection.None;
        }
        else if(gameEvent.ID == GameEventId.PathfindingData)
        {
            gameEvent.Paramters[EventParameter.BlocksMovement] = true;
            gameEvent.Paramters[EventParameter.Weight] = Pathfinder.ImpassableWeight;
        }
    }
}

public class DTO_Wall : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Wall();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Wall);
    }
}