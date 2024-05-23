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
            { Debug.Log("Unequip " + Item.name); });

            m_ContextMenu.AddMenuItem("Drop", delegate
            {
                m_Source.GetComponent<EquipmentHandler>().Unequip(Item);
                m_Source.GetComponent<Inventory>().RemoveFromInventory(Item);
                Spawner.Instance.Spawn(Item, m_Source.transform.position);
                Clear();
            });
            this.m_ContextMenu.Show();
        }
    }
}
