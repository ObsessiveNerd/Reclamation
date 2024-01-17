using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentData : ComponentData
{ 
    public BodyPartType EquipsTo;
}


public class Equipment : EntityComponent
{
    public EquipmentData Data = new EquipmentData();

    public override void WakeUp(IComponentData data = null)
    {
        if (data != null)
            Data = data as EquipmentData;
    }

    public override IComponentData GetData()
    {
        return Data;
    }
}
