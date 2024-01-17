using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DamageData : ComponentData
{
    public Dice DamageAmount;
    public DamageType Type;
}

[Serializable]
public class Damage : EntityComponent
{
    public DamageData Data = new DamageData();

    public override void WakeUp(IComponentData data = null)
    {
        if(data != null) 
            Data = data as DamageData;
    }

    public override IComponentData GetData()
    {
        return Data;
    }
}
