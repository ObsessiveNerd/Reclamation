using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MeleeWeaponData : EntityComponent
{
    [SerializeField]
    public Type MonobehaviorType = typeof(MeleeWeapon);
    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.PerformAttack, PerformAttack);
    }

    void PerformAttack(GameEvent gameEvent)
    {
        var damages = Entity.GetComponents<Damage>();
        var damageTaken = GameEventPool.Get(GameEventId.DamageTaken)
            .With(gameEvent.Paramters)
            .With(EventParameter.DamageList, damages);

        gameEvent.GetValue<GameObject>(EventParameter.Target).FireEvent(damageTaken);

        damageTaken.Release();
    }
}

public class MeleeWeapon : ComponentBehavior<MeleeWeaponData>
{
   
}