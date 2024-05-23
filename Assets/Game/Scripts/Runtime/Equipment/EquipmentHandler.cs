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
    public List<SO_Equipment> AllEquipment;
    Dictionary<Slot, SO_Item> EquipedItems = new Dictionary<Slot, SO_Item>();

    // Start is called before the first frame update
    void Start()
    {
        m_WeaponHandler = GetComponentInChildren<WeaponHandler>();
        m_Inventory = GetComponent<Inventory>();
        AllEquipment = new List<SO_Equipment>()
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
            case Slot.MainHand:
                break;

            case Slot.OffHand:
                break;

            case Slot.TwoHanded:
                break;

            case Slot.Helmet:
                SetEquipment(equipment, Helmet);
                break;

            case Slot.Chest:
                SetEquipment(equipment, Chest);
                break;

            case Slot.Gloves:
                SetEquipment(equipment, Gloves);
                break;

            case Slot.Legs:
                SetEquipment(equipment, Legs);
                break;

            case Slot.Boots:
                SetEquipment(equipment, Boots);
                break;

            case Slot.Ring:
                if (Ring1 == null)
                    SetEquipment(equipment, Ring1);
                else if (Ring2 == null)
                    SetEquipment(equipment, Ring2);
                break;
            
            case Slot.Necklace:
                SetEquipment(equipment, Necklace);
                break;
            
            case Slot.Back:
                SetEquipment(equipment, Back);
                break;
        }
    }

    public Dictionary<Slot, SO_Item> GetWeapons()
    {
        EquipedItems.Clear();
        if(m_WeaponHandler.MainHand != null)
            EquipedItems.Add(Slot.MainHand, m_WeaponHandler.MainHand);
        if(m_WeaponHandler.OffHand != null)
            EquipedItems.Add(Slot.OffHand, m_WeaponHandler.OffHand);

        return EquipedItems;
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

    public void Unequip(SO_Item equipment)
    {
        if (m_Inventory.AddToInventory(equipment))
        {
            if (equipment == m_WeaponHandler.MainHand)
                m_WeaponHandler.MainHand = null;
            if(equipment == m_WeaponHandler.OffHand)
                m_WeaponHandler.OffHand = null;
        }

    }

    public float GetResistances(DamageType damageType)
    {
        float totalPercent = 100.0f;
        foreach(var equipment in AllEquipment)
        {
            foreach(var resistance in equipment.Resistances)
            if(resistance.DamageType == damageType)
                totalPercent *= resistance.Percent;
        }
        return totalPercent / 100f;
    }
}
