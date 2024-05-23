using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Reclaim/Equipment")]
public class SO_Equipment : SO_Item
{
    public Slot Slot;
    public List<Resistance> Resistances;
    public List<Effects> OnEquip;

    public void Equip(GameObject source)
    {

    }

    public void Unequip(GameObject source) 
    {
        
    }
}
