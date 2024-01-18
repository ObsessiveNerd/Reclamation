using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class DamageData : EntityComponent
{
    public Dice DamageAmount;
    public DamageType Type;

    public override void WakeUp()
    {

    }
}

public class Damage : EntityComponentBehavior
{
    public DamageData Data = new DamageData();

    public override IComponent GetData()
    {
        return Data;
    }
}
