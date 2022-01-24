using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantmentManagerMono : EscapeableMono
{
    public GameObject View;
    public Transform Inventories;
    public EnchantItemSlotMono ItemToEnchant;
    public EnchantItemSlotMono Result;

    List<GameObject> m_Inventories;
    public void Setup(IEntity enchantment)
    {
        UIManager.Push(this);
        View.SetActive(true);
        m_Inventories = UIUtility.CreatePlayerInventories(Inventories);
    }

    public override void OnEscape()
    {
        foreach (GameObject go in m_Inventories)
        {
            go.GetComponent<InventoryManagerMono>().Close();
            Destroy(go);
        }
        m_Inventories.Clear();
        View.SetActive(false);
    }
}
