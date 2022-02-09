using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnchantmentManagerMono : EscapeableMono
{
    public GameObject View;
    public Transform Inventories;
    public EnchantItemSlotMono ItemToEnchant;
    public EnchantItemSlotMono Result;
    public TMP_InputField NewAssetName;

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

        ItemToEnchant.BeforeDrop -= NewItemDropped;
        ItemToEnchant.BeforeDrop += NewItemDropped;
        
        ItemToEnchant.Dropped -= CreateEnchantment;
        ItemToEnchant.Dropped += CreateEnchantment;

        ItemToEnchant.PickedUp -= OriginalItemClear;
        ItemToEnchant.PickedUp += OriginalItemClear;

        Result.PickedUp -= ResultClear;
        Result.PickedUp += ResultClear;
    }

    public override void OnEscape()
    {
        Clear(false);
    }

    void Clear(bool itemDroppedIntoInventory)
    {
         foreach (GameObject go in m_Inventories)
        {
            go.GetComponent<InventoryManagerMono>().Close();
            Destroy(go);
        }

        m_Inventories.Clear();
        View.SetActive(false);

        if(!itemDroppedIntoInventory && Result.ItemMono != null && ItemToEnchant.ItemMono == null)
        {
            //emergency save the item
            GameEvent addToInventory = GameEventPool.Get(GameEventId.AddToInventory)
                                        .With(EventParameters.Entity, Result.ItemMono.ItemObject.ID);
            Services.PlayerManagerService.GetActivePlayer().FireEvent(addToInventory);
            addToInventory.Release();
        }

        Result.Cleanup();
        ItemToEnchant.Cleanup();

        //UIManager.ForcePop(this);
        Services.WorldUIService.OpenInventory();
        ItemToEnchant.AcceptsDrop = true;
        NewAssetName.text = "Name...";
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
            ItemToEnchant.AcceptsDrop = false;
            GameEvent removeFromInventory = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                        .With(EventParameters.Item, ItemToEnchant.ItemMono.ItemObject.ID);
            ItemToEnchant.ItemMono.Source.FireEvent(removeFromInventory);
            removeFromInventory.Release();

            GameEvent destroyEnchantment = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                            .With(EventParameters.Item, m_Enchantment.ID);
            m_Source.FireEvent(destroyEnchantment).Release();

            if (!Result.ItemMono.ItemObject.HasComponent(typeof(Name)))
            {
                Name name = new Name(NewAssetName.text);
                name.Init(Result.ItemMono.ItemObject);
                Result.ItemMono.ItemObject.AddComponent(name);
                Result.ItemMono.ItemObject.CleanupComponents();
            }
            else
            {
                GameEvent nameNewItem = GameEventPool.Get(GameEventId.SetName)
                                    .With(EventParameters.Name, NewAssetName.text);
                Result.ItemMono.ItemObject.FireEvent(nameNewItem);
                nameNewItem.Release();
            }

            foreach (var imm in FindObjectsOfType<InventoryManagerMono>())
            {
                imm.ItemDropped += () =>
                {
                    Clear(true);
                    foreach(var im in FindObjectsOfType<InventoryManagerMono>())
                        im.ClearCallback();
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
        NewAssetName.text = ItemToEnchant.ItemMono.ItemObject.Name;
    }
}
