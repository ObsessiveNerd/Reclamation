using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentHandler : MonoBehaviour
{
    WeaponHandler m_WeaponHandler;

    public SO_Equipment Helmet;
    public SO_Equipment Chest;
    public SO_Equipment Gloves;
    public SO_Equipment Legs;
    public SO_Equipment Boots;
    public SO_Equipment Ring1;
    public SO_Equipment Ring2;
    public SO_Equipment Necklace;
    public SO_Equipment Back;

    Inventory m_Inventory;
    List<SO_Equipment> m_AllEquipment;

    // Start is called before the first frame update
    void Start()
    {
        m_Inventory = GetComponent<Inventory>();
        m_AllEquipment = new List<SO_Equipment>()
        {
            Helmet,Chest, Gloves, Legs, Boots, Ring1,
            Ring2, Necklace, Back
        };
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AutoEquip(SO_Equipment equipment)
    {
        switch (equipment.Slot)
        {
            case EquipmentSlot.MainHand:
                break;

            case EquipmentSlot.OffHand:
                break;

            case EquipmentSlot.TwoHanded:
                break;

            case EquipmentSlot.Helmet:
                SetEquipment(equipment, Helmet);
                break;

            case EquipmentSlot.Chest:
                SetEquipment(equipment, Chest);
                break;

            case EquipmentSlot.Gloves:
                SetEquipment(equipment, Gloves);
                break;

            case EquipmentSlot.Legs:
                SetEquipment(equipment, Legs);
                break;

            case EquipmentSlot.Boots:
                SetEquipment(equipment, Boots);
                break;

            case EquipmentSlot.Ring:
                if (Ring1 == null)
                    SetEquipment(equipment, Ring1);
                else if (Ring2 == null)
                    SetEquipment(equipment, Ring2);
                break;
            
            case EquipmentSlot.Necklace:
                SetEquipment(equipment, Necklace);
                break;
            
            case EquipmentSlot.Back:
                SetEquipment(equipment, Back);
                break;
        }
    }

    void SetEquipment(SO_Equipment equipment, SO_Equipment slot)
    {
        if (slot == null)
            slot = equipment;
        else
        {
            m_Inventory.AddToInventory(slot);
            slot = equipment;
        }
    }

    public void Unequip(SO_Equipment equipment)
    {
        if (m_Inventory.AddToInventory(equipment))
            equipment = null;
    }

    public float GetResistances(DamageType damageType)
    {
        float totalPercent = 100.0f;
        foreach(var equipment in m_AllEquipment)
        {
            foreach(var resistance in equipment.Resistances)
            if(resistance.DamageType == damageType)
                totalPercent *= resistance.Percent;
        }
        return totalPercent / 100f;
    }
}
