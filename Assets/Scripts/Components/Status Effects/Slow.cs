using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : Component
{
    public Slow(IEntity self)
    {
        Init(self);

        RegisteredEvents.Add(GameEventId.AlterEnergy);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameters.EnergyRegen] = ((float)gameEvent.Paramters[EventParameters.EnergyRegen]) / 2f;
    }
}
