using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantItemMono : InventoryItemMono
{
    protected override void OnClick()
    {
        foreach (var component in Source.GetComponents())
        {
            ItemObject.AddComponent(component);
        }
    }
}
