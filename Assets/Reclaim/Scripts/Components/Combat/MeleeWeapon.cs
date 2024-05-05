using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MeleeWeaponData : EntityComponent
{
    public GameObject AttackEffect;
    public float Range;

    [SerializeField]
    public Type MonobehaviorType = typeof(MeleeWeapon);
    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.PerformAttack, PerformAttack);
    }

    void PerformAttack(GameEvent gameEvent)
    {
        var worldPos = gameEvent.GetValue<Vector3>(EventParameter.Position);
        var sourcePos = gameEvent.GetValue<Vector3>(EventParameter.Source);
        
        var directionToAttack = (worldPos - sourcePos).normalized;
        var postionOfAttack = (sourcePos + directionToAttack);
        postionOfAttack.z = 0f;

        var angle = Vector2.SignedAngle(Vector2.up, directionToAttack);
        Services.Spawner.SpawnEffect(AttackEffect, postionOfAttack, Quaternion.Euler(0f, 0f, angle));

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

    private void Update()
    {
        
    }
}