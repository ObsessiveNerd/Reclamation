﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This will need to be re-worked some as there should be armor (and weapon) slots instead of adding some of these components directly to the entity
public class Armor : Component
{
    int m_Armor;
    public Armor(int armor)
    {
        m_Armor = armor;
        RegisteredEvents.Add(GameEventId.AddArmorValue);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameters.Value] = (int)gameEvent.Paramters[EventParameters.Value] + m_Armor;
    }
}

public class DTO_Armor : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        int armor = int.Parse(data);
        Component = new Armor(armor);
    }
}