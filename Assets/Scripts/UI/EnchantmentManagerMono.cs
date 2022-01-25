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
    IEntity m_Enchantment;

    public void Setup(IEntity enchantment)
    {
        UIManager.Push(this);
        View.SetActive(true);
        m_Inventories = UIUtility.CreatePlayerInventories(Inventories);
        m_Enchantment = enchantment;
        ItemToEnchant.Dropped += CreateEnchantment;
        ItemToEnchant.PickedUp += OriginalItemClear;
        Result.PickedUp += ResultClear;
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

    void OriginalItemClear()
    {
        Result.Cleanup();
    }

    void ResultClear()
    {
        ItemToEnchant.Cleanup();
    }

    void CreateEnchantment(InventoryItemMono itemMono)
    {
        IEntity baseEntity = itemMono.ItemObject;
        IEntity source = itemMono.Source;

        IEntity newObject = new Actor(baseEntity.Name);
        foreach (var comp in baseEntity.GetComponents())
            newObject.AddComponent(comp);

        GameEvent getEnchantments = GameEventPool.Get(GameEventId.GetEnchantments)
                                    .With(EventParameters.Enchantments, new List<string>());
        m_Enchantment.FireEvent(getEnchantments);
        List<string> enchantmentEntityIds = getEnchantments.GetValue<List<string>>(EventParameters.Enchantments);
        getEnchantments.Release();

        foreach(string id in enchantmentEntityIds)
        {
            IEntity e = Services.EntityMapService.GetEntity(id);
            foreach (var comp in e.GetComponents())
                newObject.AddComponent(comp);
        }

        newObject.CleanupComponents();
        Result.ItemMono = UIUtility.CreateItemGameObject(source, newObject, Result.transform).GetComponent<InventoryItemMono>();
    }
}
