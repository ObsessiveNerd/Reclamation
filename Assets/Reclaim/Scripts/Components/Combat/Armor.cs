using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ArmorData : EntityComponent
{ 
    public int ArmorAmount;

    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.GetInfo, GetInfo);
    }

    void GetInfo(GameEvent gameEvent)
    {
        var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameter.Info);
        dictionary.Add($"{nameof(Armor)}{Guid.NewGuid()}", $"Armor Value: {ArmorAmount}");
    }
}


public class Armor : EntityComponentBehavior
{
    public ArmorData Data = new ArmorData();
    
    public override IComponent GetData()
    {
        return Data;
    }
}