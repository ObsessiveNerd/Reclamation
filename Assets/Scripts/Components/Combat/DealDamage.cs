using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : Component
{
    DamageType m_DamageType;
    Dice m_Dice;

    public DealDamage(IEntity self, DamageType damageType, Dice dice)
    {
        Init(self);
        m_DamageType = damageType;
        m_Dice = dice;

        RegisteredEvents.Add(GameEventId.Attack);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        FireEvent(World.Instance.Self, new GameEvent(GameEventId.Attack, new KeyValuePair<string, object>(EventParameters.Damage, m_Dice.Roll()),
                                                                            new KeyValuePair<string, object>(EventParameters.DamageType, m_DamageType),
                                                                            new KeyValuePair<string, object> (EventParameters.TilePosition, gameEvent.Paramters[EventParameters.TilePosition])));
    }
}
