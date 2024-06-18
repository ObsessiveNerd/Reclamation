using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    MeleeArea m_MeleeArea;

    public SO_Weapon MainHand;
    public SO_Weapon OffHand;

    public SO_Equipment Helmet;
    public SO_Equipment Chest;
    public SO_Equipment Gloves;
    public SO_Equipment Legs;
    public SO_Equipment Boots;
    public SO_Equipment Ring1;
    public SO_Equipment Ring2;
    public SO_Equipment Necklace;
    public SO_Equipment Back;

    public Dictionary <Slot, SO_Item> EquipmentMap;
    Inventory m_Inventory;
    EquipmentManager m_EquipmentManager;

    EquipmentManager EquipmentManager
    {
        get
        {
            if(m_EquipmentManager == null)
                m_EquipmentManager = FindFirstObjectByType<EquipmentManager>();
            return m_EquipmentManager;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_MeleeArea = GetComponentInChildren<MeleeArea>();
        m_Inventory = GetComponent<Inventory>();

        EquipmentMap = new Dictionary<Slot, SO_Item>()
        {
            {Slot.MainHand, MainHand },
            {Slot.OffHand, OffHand },

            {Slot.Helmet, Helmet },
            {Slot.Chest, Chest },
            {Slot.Gloves, Gloves },
            {Slot.Legs, Legs },
            {Slot.Boots, Boots },
            {Slot.Ring1, Ring1 },
            {Slot.Ring2, Ring2 },
            {Slot.Necklace, Necklace },
            {Slot.Back, Back }
        };
    }

    public void PrimaryAttack()
    {
        if (MainHand != null)
            MainHand.Attack(gameObject, m_MeleeArea, Input.mousePosition);
    }

    public void SecondaryAttack()
    {

    }

    public SO_Item GetEquipedItem(Slot slot)
    {
        return EquipmentMap[slot];
    }

    public void AutoEquip(SO_Item item)
    {
        if (item is SO_Equipment)
            AutoEquip(item as SO_Equipment);
        else if (item is SO_Weapon)
            AutoEquip(item as SO_Weapon);
    }

    public void AutoEquip(SO_Weapon weapon)
    {
        SerializedWeapon sw = weapon.GetSerializedItem() as SerializedWeapon;
        Slot initialSlot = sw.ValidEquipSlots[0];
        bool equiped = false;
        foreach (var slot in sw.ValidEquipSlots)
        {
            if (slot == Slot.TwoHanded)
            {
                if(MainHand != null)
                    Unequip(MainHand);
                if(OffHand != null)
                    Unequip(OffHand);
                
                SetEquipment(weapon, Slot.MainHand);
                SetEquipment(weapon, Slot.OffHand);
                equiped = true;
                break;
            }

            if (EquipmentMap[slot] != null)
                continue;
            else
            {
                SetEquipment(weapon, slot);
                equiped = true;
                break;
            }
        }

        if (!equiped)
        {
            Unequip(EquipmentMap[initialSlot]);
            SetEquipment(weapon, initialSlot);
        }
    }

    public void AutoEquip(SO_Equipment equipment)
    {
        SerializedEquipment se = equipment.GetSerializedItem() as SerializedEquipment;
        Slot initialSlot = se.ValidEquipSlots[0];
        bool equiped = false;
        foreach (var slot in se.ValidEquipSlots)
        {
            if (EquipmentMap[slot] != null)
                continue;
            else
            {
                SetEquipment(equipment, slot);
                equiped = true;
                break;
            }
        }

        if (!equiped)
        {
            Unequip(EquipmentMap[initialSlot]);
            SetEquipment(equipment, initialSlot);
        }
    }

    void SetEquipment(SO_Equipment equipment, Slot slot)
    {
        EquipmentManager.ClearSlot(slot);
        EquipmentMap[slot] = equipment;

        switch (slot)
        {
            case Slot.Helmet:
                Helmet = equipment;
                break;
            case Slot.Chest:
                Chest = equipment;
                break;
            case Slot.Gloves:
                Gloves = equipment;
                break;
            case Slot.Legs:
                Legs = equipment;
                break;
            case Slot.Boots:
                Boots = equipment;
                break;
            case Slot.Ring1:
                Ring1 = equipment;
                break;
            case Slot.Ring2:
                Ring2 = equipment;
                break;
            case Slot.Necklace:
                Necklace = equipment;
                break;
            case Slot.Back:
                Back = equipment;
                break;
        }
    }

    void SetEquipment(SO_Weapon weapon, Slot slot)
    {
        EquipmentManager.ClearSlot(slot);
        EquipmentMap[slot] = weapon;

        switch (slot)
        {
            case Slot.MainHand:
                MainHand = weapon;
                break;
            case Slot.OffHand:
                OffHand = weapon;
                break;
            case Slot.TwoHanded:
                MainHand = weapon;
                OffHand = weapon;
                break;
        }
    }

    public void Unequip(SO_Item item)
    {
        if (item is SO_Weapon)
            Unequip(item as SO_Weapon);
        else if (item is SO_Equipment)
            Unequip(item as SO_Equipment);
    }

    public void Unequip(SO_Equipment equipment)
    {
        if (m_Inventory.CanAddItem(equipment))
        {
            foreach (var validSlot in (equipment.GetSerializedItem() as SerializedEquipment).ValidEquipSlots)
            {
                if (EquipmentMap[validSlot] == equipment)
                {
                    SetEquipment(null as SO_Equipment, validSlot);
                    m_Inventory.AddToInventory(equipment);
                    break;
                }
            }
        }
    }

    public void Unequip(SO_Weapon weapon)
    {
        if (m_Inventory.CanAddItem(weapon))
        {
            foreach (var validSlot in (weapon.GetSerializedItem() as SerializedWeapon).ValidEquipSlots)
            {
                if (validSlot == Slot.TwoHanded)
                {
                    SetEquipment(null as SO_Weapon, Slot.MainHand);
                    SetEquipment(null as SO_Weapon, Slot.OffHand);
                    m_Inventory.AddToInventory(weapon);
                    break;
                }

                if (EquipmentMap[validSlot] == weapon)
                {
                    EquipmentMap[validSlot] = null;
                    SetEquipment(null as SO_Weapon, validSlot);
                    m_Inventory.AddToInventory(weapon);
                    break;
                }
            }
        }
    }

    public float GetResistances(DamageType damageType)
    {
        float totalPercent = 100.0f;
        //foreach (var equipment in EquipedItems)
        //{
        //    var equipmentList = equipment.Value;
        //    foreach (var e in equipmentList)
        //    {
        //        var serializedEquipment = e.GetSerializedItem() as SerializedEquipment;
        //        foreach (var resistance in serializedEquipment.Resistances)
        //            if (resistance.DamageType == damageType)
        //                totalPercent *= resistance.Percent;
        //    }
        //}
        return totalPercent / 100f;
    }
}
