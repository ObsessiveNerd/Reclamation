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

    Inventory m_Inventory;
    public List<SO_Equipment> AllEquipment;
    Dictionary<Slot, SO_Item> EquipedItems = new Dictionary<Slot, SO_Item>();

    // Start is called before the first frame update
    void Start()
    {
        m_MeleeArea = GetComponentInChildren<MeleeArea>();
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

    public void AutoEquip(SO_Weapon weapon)
    {
        SerializedWeapon sw = weapon.GetSerializedItem() as SerializedWeapon;
        bool equiped = false;
        foreach (var slot in sw.ValidEquipSlots)
        {
            switch (slot)
            {
                case Slot.MainHand:
                    break;
                case Slot.OffHand:
                    break;
                case Slot.TwoHanded:
                    break;
            }
        }
    }

    public void AutoEquip(SO_Equipment equipment)
    {
        SerializedEquipment se = equipment.GetSerializedItem() as SerializedEquipment;
        switch (se.Slot)
        {
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

    void SetEquipment(SO_Equipment equipment, SO_Equipment slot)
    {
        if (slot == null)
            slot = equipment;
        else
        {
            m_Inventory.RemoveFromInventory(equipment);
            slot = equipment;
        }
    }

    public void Unequip(SO_Item equipment)
    {
        if (m_Inventory.AddToInventory(equipment))
        {

        }

    }

    public float GetResistances(DamageType damageType)
    {
        float totalPercent = 100.0f;
        foreach (var equipment in AllEquipment)
        {
            var serializedEquipment = equipment.GetSerializedItem() as SerializedEquipment;
            foreach (var resistance in serializedEquipment.Resistances)
                if (resistance.DamageType == damageType)
                    totalPercent *= resistance.Percent;
        }
        return totalPercent / 100f;
    }
}
