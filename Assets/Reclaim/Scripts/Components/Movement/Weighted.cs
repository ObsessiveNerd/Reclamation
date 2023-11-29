using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weighted : EntityComponent
{
    public float Weight;

    public Weighted(float weight)
    {
        Weight = weight;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.PathfindingData);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.PathfindingData)
        {
            gameEvent.Paramters[EventParameter.Weight] = (float)gameEvent.Paramters[EventParameter.Weight] + Weight;
        }
    }
}

public class DTO_Weighted : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string value = data.Split('=')[1];
        Component = new Weighted(float.Parse(value));
    }

    public string CreateSerializableData(IComponent component)
    {
        Weighted w = (Weighted)component;
        return $"{nameof(Weighted)}:{nameof(w.Weight)}={w.Weight}";
    }
}
