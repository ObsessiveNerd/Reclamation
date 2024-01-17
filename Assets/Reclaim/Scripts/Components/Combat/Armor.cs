using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorData : IComponentData
{ 
    public int ArmorAmount;
}


public class Armor : EntityComponent
{
    public ArmorData Data = new ArmorData();
    public override void WakeUp(IComponentData data = null)
    {
        RegisteredEvents.Add(GameEventId.GetInfo, GetInfo);
        if (data != null)
            Data = data as ArmorData;
    }

    public override IComponentData GetData()
    {
        return Data;
    }

    void GetInfo(GameEvent gameEvent)
    {
        var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameter.Info);
        dictionary.Add($"{nameof(Armor)}{Guid.NewGuid()}", $"Armor Value: {Data.ArmorAmount}");
    }
}