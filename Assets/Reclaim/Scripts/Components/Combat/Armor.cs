using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : EntityComponent
{
    public int ArmorAmount;
    public void Start()
    {
        RegisteredEvents.Add(GameEventId.GetInfo, GetInfo);
    }

    void GetInfo(GameEvent gameEvent)
    {
        var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameter.Info);
        dictionary.Add($"{nameof(Armor)}{Guid.NewGuid()}", $"Armor Value: {ArmorAmount}");
    }
}