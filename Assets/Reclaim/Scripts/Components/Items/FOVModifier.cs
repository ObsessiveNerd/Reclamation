using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVModifier : EntityComponent
{
    public int Modifier;

    public FOVModifier(int mod)
    {
        Modifier = mod;
    }

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.BeforeFOVRecalculated, BeforePOVCalculated);
    }

    void BeforePOVCalculated(GameEvent gameEvent)
    {
        int currentMod = (int)gameEvent.Paramters[EventParameter.FOVRange];
        currentMod += Modifier;
        gameEvent.Paramters[EventParameter.FOVRange] = currentMod;
    }
}
