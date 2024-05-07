using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EquipmentData : EntityComponent
{ 
    [SerializeField]
    public EquipmentSlotType EquipsTo;
    [SerializeField]
    public Type MonobehaviorType = typeof(Equipment);
}


public class Equipment : ComponentBehavior<EquipmentData>
{

}
