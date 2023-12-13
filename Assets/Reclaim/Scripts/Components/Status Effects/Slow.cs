using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : EntityComponent
{
    public void Start()
    {
        RegisteredEvents.Add(GameEventId.AlterEnergy, AlterEnergy);
    }

    public void AlterEnergy(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameter.EnergyRegen] = ((float)gameEvent.Paramters[EventParameter.EnergyRegen]) / 2f;
    }
}