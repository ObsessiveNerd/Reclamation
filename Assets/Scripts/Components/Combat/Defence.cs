using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defence : Component
{
    const int kBaseAC = 10;
    public override int Priority => 1;

    public Defence(IEntity self)
    {
        Init(self);
        RegisteredEvents.Add(GameEventId.TakeDamage);
        RegisteredEvents.Add(GameEventId.Sharpness);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        //Todo: we need to check what the weapon type is and decide if we need to get armor/resistances/other...
        int rollToHit = (int)gameEvent.Paramters[EventParameters.RollToHit];
        GameEvent getArmor = new GameEvent(GameEventId.GetArmor, new KeyValuePair<string, object>(EventParameters.Value, 0));
        int armorBonus = (int)FireEvent(Self, getArmor).Paramters[EventParameters.Value];

        if (gameEvent.ID == GameEventId.TakeDamage)
        {
            if (rollToHit >= kBaseAC + armorBonus)
                Debug.Log($"{Self.Name} was hit!");
            else
            {
                Debug.Log($"Attack missed because armor was {kBaseAC + armorBonus}!");
                gameEvent.Paramters[EventParameters.Damage] = 0;
            }
        }

        if (gameEvent.ID == GameEventId.Sharpness)
        {
            if (rollToHit < kBaseAC + armorBonus)
                Debug.Log("Nothing was severed.");
            else
                FireEvent(Self, new GameEvent(GameEventId.SeverBodyPart));
        }
    }
}
