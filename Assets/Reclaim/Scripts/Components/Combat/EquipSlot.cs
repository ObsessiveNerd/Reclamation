using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum EquipmentSlotType
{
    Helmet,
    Chest,
    Pants,
    Gloves,
    Boots,
    Ring1,
    Ring2,
    Necklace,
    Cape,
    MainHand,
    OffHand
}

[Serializable]
public class EquipSlot
{
    public GameObject Equipment;
    //public EquipmentSlotType EquipmentSlot;
    
    public bool CanEquip {get{ return Equipment == null; }}

    public void Activate()
    {
        if (Equipment != null)
            Equipment.GetComponent<EntityBehavior>().Activate();
    }

    public void FireEvent(GameEvent gameEvent)
    {
        if (Equipment != null)
            Equipment.FireEvent(gameEvent);
    }
}