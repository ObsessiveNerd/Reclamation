using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : UISlotBase
{
    public Slot Slot;
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            this.m_ContextMenu.Clear();
            m_ContextMenu.AddMenuItem("Unequip", delegate
            {
                Unequip();
            });

            m_ContextMenu.AddMenuItem("Drop", delegate
            {
                m_Source.GetComponent<Equipment>().Unequip(Item);
                m_Source.GetComponent<Inventory>().RemoveFromInventory(Item);
                Spawner.Instance.Spawn(Item, m_Source.transform.position);
            });
            this.m_ContextMenu.Show();
        }

        if (eventData.clickCount == 1)
            Unequip();
    }

    void Unequip()
    {
        m_Source.GetComponent<Equipment>().Unequip(Item);
        FindFirstObjectByType<InventoryManager>().Open(m_Source);
    }
}
