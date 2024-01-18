using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EquipmentData : EntityComponent
{ 
    public BodyPartType EquipsTo;
    public override Type MonobehaviorType => typeof(Equipment);
}


public class Equipment : ComponentBehavior<EquipmentData>
{

}
