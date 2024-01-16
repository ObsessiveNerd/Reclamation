using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeWeapon : EntityComponent
{
    void Start()
    {
        RegisteredEvents.Add(GameEventId.PerformAttack, PerformAttack);
    }

    void PerformAttack(GameEvent gameEvent)
    {
        var damages = GetComponents<Damage>();
        var damageTaken = GameEventPool.Get(GameEventId.DamageTaken)
            .With(gameEvent.Paramters)
            .With(EventParameter.DamageList, damages);

        gameEvent.GetValue<GameObject>(EventParameter.Target).FireEvent(damageTaken);

        damageTaken.Release();
    }
}