using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int InventoryLimit;

    GameObject[] m_Inventory;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Inventory = new GameObject[InventoryLimit];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool AddToInventory(GameObject inventoryItem)
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

    public bool AddToInventorySpecific(GameObject inventoryItem, int index)
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

    public void RemoveFromInventory(GameObject inventoryItem)
    {
        for(int i = 0; i < InventoryLimit; i++)
        {
            if (m_Inventory[i] == inventoryItem)
                m_Inventory[i] = null;
        }
    }

    int GetIndex(GameObject inventoryItem)
    {
        for(int i = 0; i < InventoryLimit; i++)
        {
            if (m_Inventory[i] == inventoryItem)
                return i;
        }
        return -1;
    }
}
