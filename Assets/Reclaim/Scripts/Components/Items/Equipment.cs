using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EquipmentData : EntityComponent
{ 
    public BodyPartType EquipsTo;
}


public class Equipment : EntityComponentBehavior
{
    public EquipmentData Data = new EquipmentData();

    public override IComponent GetData()
    {
        return Data;
    }
}
