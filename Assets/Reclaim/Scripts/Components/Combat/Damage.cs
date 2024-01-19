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

    }
}

public class Damage : ComponentBehavior<DamageData>
{
    
}
