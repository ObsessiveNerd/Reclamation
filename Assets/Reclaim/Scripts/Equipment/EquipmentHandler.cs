using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentHandler : MonoBehaviour
{
    WeaponHandler m_WeaponHandler;

    public Equipment Helmet;
    public Equipment Chest;
    public Equipment Gloves;
    public Equipment Legs;
    public Equipment Boots;
    public Equipment Ring1;
    public Equipment Ring2;
    public Equipment Necklace;
    public Equipment Back;

    Inventory m_Inventory;
    List<Equipment> m_AllEquipment;

    // Start is called before the first frame update
    void Start()
    {
        m_Inventory = GetComponent<Inventory>();
        m_AllEquipment = new List<Equipment>()
        {
            Helmet,Chest, Gloves, Legs, Boots, Ring1,
            Ring2, Necklace, Back
        };
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AutoEquip(Equipment equipment)
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

    void SetEquipment(Equipment equipment, Equipment slot)
    {
        if (slot == null)
            slot = equipment;
        else
        {
            m_Inventory.AddToInventory(slot.gameObject);
            slot = equipment;
        }
    }

    public void Unequip(Equipment equipment)
    {
        if (m_Inventory.AddToInventory(equipment.gameObject))
            equipment = null;
    }

    public float GetResistances(DamageType damageType)
    {
        float totalPercent = 100.0f;
        foreach(var equipment in m_AllEquipment)
        {
            var resistance = equipment?.GetComponent<Resistance>();
            if(resistance != null && resistance.DamageType == damageType)
                totalPercent *= resistance.Percent;
        }
        return totalPercent;
    }
}
