using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[Serializable]
public class BodyData : EntityComponent
{
    public EquipSlot Helmet;
    public EquipSlot Chest;
    public EquipSlot Pants;
    public EquipSlot Gloves;
    public EquipSlot Boots;
    public EquipSlot Ring1;
    public EquipSlot Ring2;
    public EquipSlot Necklace;
    public EquipSlot Cape;

    Type MonobehaviorType = typeof(Body);

    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.ApplyDamage, DamageTaken);

        Helmet.Activate();
        Chest.Activate();
        Pants.Activate();
        Gloves.Activate();
        Boots.Activate();
        Ring1.Activate();
        Ring2.Activate();
        Necklace.Activate();
        Cape.Activate();
    }

    void DamageTaken(GameEvent gameEvent)
    {
        
    }
}


public class Body : ComponentBehavior<BodyData>
{

}
