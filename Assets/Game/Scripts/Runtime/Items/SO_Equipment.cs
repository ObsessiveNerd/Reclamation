using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedEquipment : SerializedItem
{
    public List<Slot> ValidEquipSlots;
    public List<Resistance> Resistances;
    
    [SerializeReference, Subclass(IsList = true)]
    public List<Effects> OnEquipEffects;
}

[CreateAssetMenu(fileName = "Equipment", menuName = "Reclaim/Equipment")]
public class SO_Equipment : SO_Item
{
    [SerializeField]
    private SerializedEquipment SerializedEquipment1;

    public void Equip(GameObject source)
    {

    }

    public void Unequip(GameObject source)
    {

    }

    public override SerializedItem GetSerializedItem()
    {
        return SerializedEquipment1;
    }
}
