using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public GameObject EquipmentUI;

    GameObject m_Source;
    Dictionary<Slot, List<EquipmentSlot>> m_Slots = new Dictionary<Slot, List<EquipmentSlot>>();

    public void Open(GameObject source)
    {
        m_Source = source;
        SetupDictionary();

        var equipHandler = m_Source.GetComponent<EquipmentHandler>();
        foreach (var item in equipHandler.GetWeapons())
        {
            m_Slots[item.Key][0].SetItem(source, item.Value);
        }

        EquipmentUI.SetActive(true);
    }

    void SetupDictionary()
    {
        foreach (var slot in EquipmentUI.GetComponentsInChildren<EquipmentSlot>())
        {
            Slot s = slot.Slot;
            if (m_Slots.ContainsKey(s))
                m_Slots[s].Add(slot);
            else
                m_Slots.Add(s, new List<EquipmentSlot>() { slot });
        }
    }

    public void Close()
    {
        EquipmentUI.SetActive(false);
    }
}
