using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum BodyPartType
{
    None,
    Head,
    Torso,
    Arm,
    Leg,
    Finger,
    Back,
    Neck
}

[Serializable]
public class BodyPart
{
    public GameObject Equipment;
    public BodyPartType BodyPartType;
    
    public bool CanEquip {get{ return Equipment == null; }}

    public void Activate()
    {
        if (Equipment != null)
            Equipment = Services.EntityFactory.Create(Equipment);
    }

    public void PassEventToEquipment(GameEvent gameEvent)
    {
        if (Equipment != null)
            Equipment.FireEvent(gameEvent);
    }
}