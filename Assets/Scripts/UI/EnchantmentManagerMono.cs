using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantmentManagerMono : EscapeableMono
{
    public Transform Inventories;
    public EnchantItemSlotMono ItemToEnchant;
    public EnchantItemSlotMono Result;

    public void Setup(IEntity enchantment)
    {
        UIManager.Push(this);
        GameObject inventory = Resources.Load<GameObject>("UI/Inventory");
        foreach (string id in Services.WorldDataQuery.GetPlayableCharacters())
        {
            GameObject go = Instantiate(inventory);
            go.GetComponent<InventoryManagerMono>().Setup(Services.EntityMapService.GetEntity(id));
            go.transform.SetParent(Inventories, false);
        }
    }
}
