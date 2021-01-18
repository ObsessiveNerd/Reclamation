using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : Component
{
    public Slow()
    {
        RegisteredEvents.Add(GameEventId.AlterEnergy);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameters.EnergyRegen] = ((float)gameEvent.Paramters[EventParameters.EnergyRegen]) / 2f;
    }
}

public class DTO_Slow : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Slow();
    }

    public string CreateSerializableData(IComponent comp)
    {
        return nameof(Slow);
    }
}