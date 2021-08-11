using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This will need to be re-worked some as there should be armor (and weapon) slots instead of adding some of these components directly to the entity
public class Armor : Component
{
    public int ArmorAmount;
    public Armor(int armor)
    {
        ArmorAmount = armor;
        RegisteredEvents.Add(GameEventId.AddArmorValue);
        RegisteredEvents.Add(GameEventId.GetCombatRating);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.AddArmorValue || gameEvent.ID == GameEventId.GetCombatRating)
            gameEvent.Paramters[EventParameters.Value] = (int)gameEvent.Paramters[EventParameters.Value] + ArmorAmount;
    }
}

public class DTO_Armor : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string value = data;
        if (value.Contains("="))
            value = value.Split('=')[1];
        int armor = int.Parse(value);
        Component = new Armor(armor);
    }

    public string CreateSerializableData(IComponent component)
    {
        Armor a = (Armor)component;
        return $"{nameof(Armor)}:{nameof(a.ArmorAmount)}={a.ArmorAmount}";
    }
}