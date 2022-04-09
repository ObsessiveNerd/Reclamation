using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class InventoryManagerMono : UpdatableUI, IDropHandler
{
    public Action ItemDropped;
    public TextMeshProUGUI Name;
    public Transform InventoryView;
    
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
        foreach (IEntity entity in m_Items.Keys)
        {
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
            {
                if(item.HasComponent(typeof(Stackable)))
                {
                    if(!m_Items.Keys.Any(k => k.Name == item.Name))
                       m_Items.Add(item, UIUtility.CreateItemGameObject(Source, item, InventoryView));
                }
                else
                    m_Items.Add(item, UIUtility.CreateItemGameObject(Source, item, InventoryView));
            }
        }
    }

    public void Close()
    {
        Cleanup();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null || eventData.pointerDrag.GetComponent<InventoryItemMono>() == null)
            return;

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
        Services.WorldUIService.UpdateUI();
    }
}
