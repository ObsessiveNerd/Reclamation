using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : Component
{
    const int kBaseAC = 10;
    public override int Priority => 1;

    public Defense()
    {
        RegisteredEvents.Add(GameEventId.TakeDamage);
        RegisteredEvents.Add(GameEventId.Sharpness);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        //Todo: we need to check what the weapon type is and decide if we need to get armor/resistances/other...
        int rollToHit = (int)gameEvent.Paramters[EventParameters.RollToHit];
        GameEvent getArmor = new GameEvent(GameEventId.AddArmorValue, new KeyValuePair<string, object>(EventParameters.Value, 0));
        int armorBonus = (int)FireEvent(Self, getArmor).Paramters[EventParameters.Value];

        if (gameEvent.ID == GameEventId.TakeDamage)
        {
            if (rollToHit >= kBaseAC + armorBonus)
                RecLog.Log($"{Self.Name} was hit!");
            else
            {
                RecLog.Log($"Attack missed because armor was {kBaseAC + armorBonus}!");
                gameEvent.ContinueProcessing = false;
            }
        }

        if (gameEvent.ID == GameEventId.Sharpness)
        {
            if (rollToHit < kBaseAC + armorBonus)
                RecLog.Log("Nothing was severed.");
            else
                FireEvent(Self, new GameEvent(GameEventId.SeverBodyPart));
        }
    }
}

public class DTO_Defense : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Defense();
    }

    public string CreateSerializableData(IComponent comp)
    {
        return nameof(Defense);
    }
}
