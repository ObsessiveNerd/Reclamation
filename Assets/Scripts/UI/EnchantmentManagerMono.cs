using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantmentManagerMono : EscapeableMono
{
    public Transform Inventories;
    public EnchantItemSlotMono ItemToEnchant;
    public EnchantItemSlotMono Result;

    List<GameObject> m_Inventories;
    public void Setup(IEntity enchantment)
    {
        UIManager.Push(this);
        m_Inventories = UIUtility.CreatePlayerInventories(Inventories);
    }

    public override void OnEscape()
    {
        foreach (GameObject go in m_Inventories)
            Destroy(go);
    }
}
