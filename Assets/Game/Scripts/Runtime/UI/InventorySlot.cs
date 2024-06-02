using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DevionGames.UIWidgets;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ContextMenu = DevionGames.UIWidgets.ContextMenu;

public class InventorySlot : UISlotBase
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            this.m_ContextMenu.Clear();
            m_ContextMenu.AddMenuItem("Equip", delegate
            {
                Equip();
            });

            m_ContextMenu.AddMenuItem("Drop", delegate
            {
                m_Source.GetComponent<Inventory>().RemoveFromInventory(Item);
                Spawner.Instance.Spawn(Item, m_Source.transform.position);
                Clear();
            });
            this.m_ContextMenu.Show();
        }

        if (eventData.clickCount == 1)
            Equip();
    }

    void Equip()
    {
        m_Source.GetComponent<Equipment>().AutoEquip(Item);
        m_Source.GetComponent<Inventory>().RemoveFromInventory(Item);
        FindFirstObjectByType<EquipmentManager>().Open(m_Source);
        FindFirstObjectByType<InventoryManager>().Open(m_Source);
        Clear();
    }
}
