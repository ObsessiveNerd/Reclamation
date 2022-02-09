using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryManagerMono : UpdatableUI, IDropHandler
{
    public Action ItemDropped;
    public TextMeshProUGUI Name;
    Transform m_InventoryView;
    public Transform InventoryView
    {
        get
        {
            if (m_InventoryView == null)
                m_InventoryView = transform.Find("InventoryView").transform;
            return m_InventoryView;
        }
    }
    public IEntity Source;
    Dictionary<IEntity, GameObject> m_Items = new Dictionary<IEntity,GameObject>();

    public void ClearCallback()
    {
        ItemDropped = null;
    }

    public void Setup(IEntity source)
    {
        if (source != null)
            Source = source;
    }

    public void Cleanup()
    {
        List<IEntity> keysToRemove = new List<IEntity>();
        var enchanter = FindObjectOfType<EnchantmentManagerMono>();
        foreach (IEntity entity in m_Items.Keys)
        {
            //This is hacky and awful.  If we ever need to do this in another situation we'll have to refactor this whole thing
            //The proper thing would be for things like the enchanter to create a temporary inventory entity that has an inventory
            //Then we move the item to be enchanted into that inventory while we're enchanting that item
            if (enchanter != null)
            {
                if (enchanter.ItemToEnchant.ItemMono != null &&
                    enchanter.ItemToEnchant.ItemMono.ItemObject == entity)
                    continue;
            }

            Destroy(m_Items[entity]);
            keysToRemove.Add(entity);
        }

        foreach (var key in keysToRemove)
            m_Items.Remove(key);
    }

    public override void UpdateUI()
    {
        Cleanup();

        Name.text = Source.Name;

        GameEvent getCurrentInventory = GameEventPool.Get(GameEventId.GetCurrentInventory)
                                            .With(EventParameters.Value, new List<IEntity>());

        List<IEntity> inventory = Source.FireEvent(getCurrentInventory).GetValue<List<IEntity>>(EventParameters.Value);
        getCurrentInventory.Release();

        foreach (var item in inventory)
        {
            if (!m_Items.ContainsKey(item))
                m_Items.Add(item, UIUtility.CreateItemGameObject(Source, item, InventoryView));
        }
    }

    public void Close()
    {
        Cleanup();
    }

    public void OnDrop(PointerEventData eventData)
    {
        IEntity source = eventData.pointerDrag.GetComponent<InventoryItemMono>().Source;
        IEntity item = eventData.pointerDrag.GetComponent<InventoryItemMono>().ItemObject;

        GameEvent getBodyPartForEquipment = GameEventPool.Get(GameEventId.GetBodyPartType)
            .With(EventParameters.BodyPart, BodyPart.None);
        item.FireEvent(getBodyPartForEquipment);
        BodyPart equipmentBodyPart = getBodyPartForEquipment.GetValue<BodyPart>(EventParameters.BodyPart);
        getBodyPartForEquipment.Release();

        if (equipmentBodyPart != BodyPart.None)
        {
            GameEvent unEquip = GameEventPool.Get(GameEventId.Unequip)
                .With(EventParameters.Entity, source.ID)
                .With(EventParameters.EntityType, equipmentBodyPart)
                .With(EventParameters.Item, item.ID);

            source.FireEvent(unEquip);
            unEquip.Release();
        }

        if (source != Source)
        {
            GameEvent removeFromInventory = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                        .With(EventParameters.Item, item.ID);
            source.FireEvent(removeFromInventory);
            removeFromInventory.Release();
            //Services.WorldUIService.UpdateUI(source.ID);
        }

        GameEvent addToInventory = GameEventPool.Get(GameEventId.AddToInventory)
                                    .With(EventParameters.Entity, item.ID);
        Source.FireEvent(addToInventory);
        addToInventory.Release();

        //eventData.pointerDrag.GetComponent<InventoryItemMono>().Init(Source, item);
        //eventData.pointerDrag.GetComponent<DragAndDrop>().Set(InventoryView.position, InventoryView.transform);

        Services.WorldUIService.UpdateUI();

        if (!m_Items.ContainsKey(item))
        {
            Debug.Log($"Item {item.Name} is getting added to inventory");
            m_Items.Add(item, eventData.pointerDrag);
        }
        else
        {
            Debug.Log($"Destroy {item.Name}");
            Destroy(eventData.pointerDrag);
        }

        ItemDropped?.Invoke();
    }

    public void UpdateUI(IEntity newSource)
    {
        Cleanup();
        Setup(Source);
    }
}
