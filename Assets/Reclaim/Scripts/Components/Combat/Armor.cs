using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ArmorData : EntityComponent
{ 
    [SerializeField]
    public int ArmorAmount;
    [SerializeField]
    public Type MonobehaviorType = typeof(Armor);
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


public class Armor : ComponentBehavior<ArmorData>
{

}