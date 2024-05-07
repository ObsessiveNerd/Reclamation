using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class DamageData : EntityComponent
{
    [SerializeField]
    public Dice DamageAmount;
    [SerializeField]
    public DamageType Type;

    public Type MonobehaviorType = typeof(Damage);

    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.PerformAttack, AddDamage);
    }

    void AddDamage(GameEvent gameEvent)
    {
        gameEvent.GetValue<List<DamageData>>(EventParameter.DamageList).Add(this);
    }
}

public class Damage : ComponentBehavior<DamageData>
{
    
}
