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

    InventoryManager m_Manager;
    InventoryManager Manager
    {
        get
        {
            if(m_Manager == null)
                m_Manager = FindFirstObjectByType<InventoryManager>();
            return m_Manager;
        }
    }

    bool m_IsOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        m_Inventory = new SO_Item[InventoryLimit];
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if(Input.GetKeyDown(KeyCode.I))
        {
            if (!m_IsOpen)
                Manager.Open(gameObject);
            else
                Manager.Close();
            m_IsOpen = !m_IsOpen;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            Manager.Close();
            m_IsOpen = false;
        }
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
