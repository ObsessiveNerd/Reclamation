using System;
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
    GameObject m_Enchantment;
    GameObject m_Source;

    bool m_IsClearing = false;

    //public void Setup(GameObject source, GameObject enchantment)
    //{
    //    View.SetActive(true);
    //    m_Inventories = UIUtility.CreatePlayerInventories(Inventories);
    //    m_Enchantment = enchantment;
    //    m_Source = source;

    //    ItemToEnchant.BeforeDrop -= NewItemDropped;
    //    ItemToEnchant.BeforeDrop += NewItemDropped;

    //    ItemToEnchant.Dropped -= CreateEnchantment;
    //    ItemToEnchant.Dropped += CreateEnchantment;

    //    ItemToEnchant.PickedUp -= OriginalItemClear;
    //    ItemToEnchant.PickedUp += OriginalItemClear;

    //    Result.PickedUp -= ResultClear;
    //    Result.PickedUp += ResultClear;
    //    m_IsClearing = false;
    //}

    //public override void OnEscape()
    //{
    //    Clear(false);
    //    gameObject.SetActive(false);  
    //}

    //void Clear(bool itemDroppedIntoInventory)
    //{
    //    if (m_IsClearing)
    //        return;
    //    m_IsClearing = true;

    //    foreach (GameObject go in m_Inventories)
    //    {
    //        go.GetComponent<InventoryManagerMono>().Close();
    //        Destroy(go);
    //    }

    //    m_Inventories.Clear();

    //    View.SetActive(false);

    //    Result.Cleanup();
    //    ItemToEnchant.Cleanup();

    //    if (!itemDroppedIntoInventory && Result.ItemMono != null && ItemToEnchant.ItemMono == null)
    //    {
    //        //emergency save the item
    //        GameEvent addToInventory = GameEventPool.Get(GameEventId.AddToInventory)
    //                                    .With(EventParameter.Entity, Result.ItemMono.ItemObject.ID);
    //        Services.PlayerManagerService.GetActivePlayer().FireEvent(addToInventory);
    //        addToInventory.Release();
    //    }

    //    Services.WorldUIService.OpenInventory();
    //    ItemToEnchant.AcceptsDrop = true;
    //    NewAssetName.text = "Name...";
    //}

    //void OriginalItemClear()
    //{
    //    if (Result.ItemMono != null)
    //        Destroy(Result.ItemMono.gameObject);
    //}

    //void ResultClear()
    //{
    //    if (ItemToEnchant.ItemMono != null)
    //    {
    //        ItemToEnchant.AcceptsDrop = false;
    //        GameEvent removeFromInventory = GameEventPool.Get(GameEventId.RemoveFromInventory)
    //                                    .With(EventParameter.Item, ItemToEnchant.ItemMono.ItemObject.ID);
    //        ItemToEnchant.ItemMono.Source.FireEvent(removeFromInventory);
    //        removeFromInventory.Release();

    //        GameEvent destroyEnchantment = GameEventPool.Get(GameEventId.RemoveFromInventory)
    //                                        .With(EventParameter.Item, m_Enchantment.ID);
    //        m_Source.FireEvent(destroyEnchantment).Release();

    //        if (!Result.ItemMono.ItemObject.HasComponent(typeof(Name)))
    //        {
    //            Name name = new Name(NewAssetName.text);
    //            name.Init(Result.ItemMono.ItemObject);
    //            Result.ItemMono.ItemObject.AddComponent(name);
    //            Result.ItemMono.ItemObject.CleanupComponents();
    //        }
    //        else
    //        {
    //            GameEvent nameNewItem = GameEventPool.Get(GameEventId.SetName)
    //                                .With(EventParameter.Name, NewAssetName.text);
    //            Result.ItemMono.ItemObject.FireEvent(nameNewItem);
    //            nameNewItem.Release();
    //        }

    //        foreach (var imm in FindObjectsOfType<InventoryManagerMono>())
    //        {
    //            imm.ItemDropped += () =>
    //            {
    //                Clear(true);
    //                foreach (var im in FindObjectsOfType<InventoryManagerMono>())
    //                    im.ClearCallback();
    //            };
    //        }

    //        Destroy(ItemToEnchant.ItemMono.gameObject);
    //    }
    //}
    //void NewItemDropped(InventoryItemMono itemMono)
    //{
    //    if (itemMono != null && Result.ItemMono != null)
    //    {
    //        foreach (var imm in FindObjectsOfType<InventoryManagerMono>())
    //        {
    //            if (imm.Source == itemMono.Source)
    //                itemMono.transform.SetParent(imm.InventoryView, false);
    //        }
    //        Destroy(Result.ItemMono.gameObject);
    //        Result.ItemMono = null;
    //    }
    //}

    //void CreateEnchantment(InventoryItemMono itemMono)
    //{
    //    GameObject baseEntity = itemMono.ItemObject;
    //    GameObject source = itemMono.Source;

    //    GameObject newObject = new Actor(baseEntity.Name);
    //    foreach (var comp in baseEntity.GetComponents())
    //        newObject.AddComponent(comp);

    //    GameEvent getEnchantments = GameEventPool.Get(GameEventId.GetEnchantments)
    //                                .With(EventParameter.Enchantments, new List<string>());
    //    m_Enchantment.FireEvent(getEnchantments);
    //    List<string> enchantmentEntityIds = getEnchantments.GetValue<List<string>>(EventParameter.Enchantments);
    //    getEnchantments.Release();

    //    foreach (string id in enchantmentEntityIds)
    //    {
    //        GameObject e = Services.EntityMapService.GetEntity(id);
    //        foreach (var comp in e.GetComponents())
    //            newObject.AddComponent(comp);
    //    }

    //    newObject.CleanupComponents();
    //    Result.ItemMono = UIUtility.CreateItemGameObject(source, newObject, Result.transform).GetComponent<InventoryItemMono>();
    //    Result.ItemMono.AllowConxtMenuOptions = false;
    //    NewAssetName.text = ItemToEnchant.ItemMono.ItemObject.Name;
    //}
}
