using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Immunity : EntityComponent
{
    public DamageType Type;
    public Immunity(DamageType type)
    {
        Type = type;
    }

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.GetImmunity, GetImmunity);
        RegisteredEvents.Add(GameEventId.TakeDamage, TakeDamage);
    }

    void GetImmunity(GameEvent gameEvent)
    {
        gameEvent.GetValue<List<DamageType>>(EventParameter.Immunity).Add(Type);
    }

    void TakeDamage(GameEvent gameEvent)
    {
        foreach (var damage in gameEvent.GetValue<List<Damage>>(EventParameter.DamageList))
        {
            if (damage.DamageType == Type)
                damage.DamageAmount = 0;
        }
    }
}
