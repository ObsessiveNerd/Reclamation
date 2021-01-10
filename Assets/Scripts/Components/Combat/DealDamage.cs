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

        RegisteredEvents.Add(GameEventId.AmAttacking);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        ((List<Damage>)gameEvent.Paramters[EventParameters.DamageList]).Add(new Damage(m_Dice.Roll(), m_DamageType));
    }
}
