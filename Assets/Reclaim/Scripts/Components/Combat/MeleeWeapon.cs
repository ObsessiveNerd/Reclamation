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
        RegisteredEvents.Add(GameEventId.SpawnEffect, SpawnEffect);
    }

    void SpawnEffect(GameEvent gameEvent)
    {
        var worldPos = gameEvent.GetValue<Vector3>(EventParameter.Position);
        var angle = gameEvent.GetValue<float>(EventParameter.Angle);
        Services.Spawner.SpawnEffect(AttackEffect, worldPos, Quaternion.Euler(0f, 0f, angle));
    }
}

public class MeleeWeapon : ComponentBehavior<MeleeWeaponData>
{

    private void Update()
    {
        
    }
}