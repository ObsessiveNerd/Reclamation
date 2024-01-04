using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : EntityComponent
{
    public int ArmorAmount;
    public void Start()
    {
        RegisteredEvents.Add(GameEventId.AddArmorValue, AddArmorValue);
        RegisteredEvents.Add(GameEventId.GetInfo, GetInfo);
    }

    void AddArmorValue(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameter.Value] = (int)gameEvent.Paramters[EventParameter.Value] + ArmorAmount;
    }

    void GetInfo(GameEvent gameEvent)
    {
        var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameter.Info);
        dictionary.Add($"{nameof(Armor)}{Guid.NewGuid()}", $"Armor Value: {ArmorAmount}");
    }
}