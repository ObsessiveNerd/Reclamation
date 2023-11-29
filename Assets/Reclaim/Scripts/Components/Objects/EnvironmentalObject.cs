using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalObject : EntityComponent
{
    public EnvironmentalObject()
    {
        RegisteredEvents.Add(GameEventId.PathfindingData);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.PathfindingData)
        {
            gameEvent.Paramters[EventParameter.BlocksMovement] = true;
            gameEvent.Paramters[EventParameter.Weight] = Pathfinder.ImpassableWeight;
        }
    }
}

public class DTO_EnvironmentalObject : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new EnvironmentalObject();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(EnvironmentalObject);
    }
}
