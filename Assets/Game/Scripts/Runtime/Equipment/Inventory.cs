using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    public int InventoryLimit;

    [SerializeField]
    SO_Item[] m_Inventory;

    // Start is called before the first frame update
    void Start()
    {
        m_Inventory = new SO_Item[InventoryLimit];
    }

    public Dictionary<int, SO_Item> GetInventory()
    {
        Dictionary<int, SO_Item> inventory = new Dictionary<int, SO_Item>();
        for(int i = 0; i < InventoryLimit; i++)
        {
            if (m_Inventory[i] != null)
                inventory.Add(i, m_Inventory[i]);
        }
        return inventory;
    }

    public bool AddToInventory(SO_Item inventoryItem)
    {
        for(int i = 0; i < InventoryLimit; i++)
        {
            if (m_Inventory[i] == null)
            { 
                m_Inventory[i] = inventoryItem;
                return true;
            }
        }
        return false;
    }

    public bool CanAddItem(SO_Item item)
    {
        return true;
    }

    public bool AddToInventorySpecific(SO_Item inventoryItem, int index)
    {
        if(m_Inventory[index] == null)
        {
            m_Inventory[index] = inventoryItem;
            return true;
        }
        else
        {
            int swapIndex = GetIndex(m_Inventory[index]);
            if(swapIndex != -1)
            {
                var temp = m_Inventory[swapIndex];
                m_Inventory[swapIndex] = inventoryItem;
                m_Inventory[index] = temp;
                return true;
            }
        }
        return false;
    }

    public void RemoveFromInventory(SO_Item inventoryItem)
    {
        for(int i = 0; i < InventoryLimit; i++)
        {
            if (m_Inventory[i] == inventoryItem)
                m_Inventory[i] = null;
        }
    }

    int GetIndex(SO_Item inventoryItem)
    {
        for(int i = 0; i < InventoryLimit; i++)
        {
            if (m_Inventory[i] == inventoryItem)
                return i;
        }
        return -1;
    }
}
