using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : EntityComponent
{
    public DamageType DamageType;
    
    [SerializeField]
    public Dice Dice;

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.AmAttacking, AmAttacking);
        RegisteredEvents.Add(GameEventId.GetInfo, GetInfo);
    }

    void AmAttacking(GameEvent gameEvent)
    {
        ((List<Damage>)gameEvent.Paramters[EventParameter.DamageList]).Add(new Damage(Dice.Roll(), DamageType));

    }

    void GetInfo(GameEvent gameEvent)
    {
        var dictionary = gameEvent.GetValue<Dictionary<string, string>>(EventParameter.Info);
        dictionary.Add($"{nameof(DealDamage)}{Guid.NewGuid()}", $"Damage Type: {DamageType}\nDamage: {Dice.GetNotation()}");
    }
}
