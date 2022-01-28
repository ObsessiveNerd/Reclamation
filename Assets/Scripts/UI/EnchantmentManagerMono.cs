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
    IEntity m_Source;

    public void Setup(IEntity source, IEntity enchantment)
    {
        UIManager.Push(this);
        View.SetActive(true);
        m_Inventories = UIUtility.CreatePlayerInventories(Inventories);
        m_Enchantment = enchantment;
        m_Source = source;

        ItemToEnchant.BeforeDrop += NewItemDropped;
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
        if(Result.ItemMono != null)
            Destroy(Result.ItemMono.gameObject);
    }

    void ResultClear()
    {
        if(ItemToEnchant.ItemMono != null)
        {
            GameEvent removeFromInventory = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                        .With(EventParameters.Item, ItemToEnchant.ItemMono.ItemObject.ID);
            ItemToEnchant.ItemMono.Source.FireEvent(removeFromInventory);
            removeFromInventory.Release();

            GameEvent destroyEnchantment = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                            .With(EventParameters.Item, m_Enchantment.ID);
            m_Source.FireEvent(destroyEnchantment).Release();

            foreach(var imm in FindObjectsOfType<InventoryManagerMono>())
            {
                imm.ItemDropped += () =>
                {

                    UIManager.ForcePop(this);
                    Services.WorldUIService.UpdateUI();
                };
            }

            Destroy(ItemToEnchant.ItemMono.gameObject);
        }
    }
    void NewItemDropped(InventoryItemMono itemMono)
    {
        if(itemMono != null && Result.ItemMono != null)
        {
            foreach(var imm in FindObjectsOfType<InventoryManagerMono>())
            {
                if (imm.Source == itemMono.Source)
                    itemMono.transform.SetParent(imm.InventoryView, false);
            }
            Destroy(Result.ItemMono.gameObject);
            Result.ItemMono = null;
        }
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
