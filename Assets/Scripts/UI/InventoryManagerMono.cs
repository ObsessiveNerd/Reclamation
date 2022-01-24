using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryManagerMono : MonoBehaviour, IDropHandler//, IUpdatableUI
{
    public Transform InventoryView;
    public IEntity Source;
    Dictionary<IEntity, GameObject> m_Items = new Dictionary<IEntity,GameObject>();

    public void Setup(IEntity source)
    {
        if (source != null)
            Source = source;

        Cleanup();

        //WorldUtility.RegisterUI(this);
        GameEvent getCurrentInventory = GameEventPool.Get(GameEventId.GetCurrentInventory)
                                            .With(EventParameters.Value, new List<IEntity>());

        List<IEntity> inventory = Source.FireEvent(getCurrentInventory).GetValue<List<IEntity>>(EventParameters.Value);
        getCurrentInventory.Release();

        foreach (var item in inventory)
            m_Items.Add(item, UIUtility.CreateItemGameObject(Source, item, InventoryView));
    }

    public void Cleanup()
    {
        foreach (GameObject go in m_Items.Values)
            Destroy(go);
        m_Items.Clear();
    }

    public void Close()
    {
        Cleanup();
        //WorldUtility.UnRegisterUI(this);
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
                                        .With(EventParameters.Entity, item.ID);
            source.FireEvent(removeFromInventory);
            removeFromInventory.Release();
            Services.WorldUIService.UpdateUI(source.ID);
        }

        GameEvent addToInventory = GameEventPool.Get(GameEventId.AddToInventory)
                                    .With(EventParameters.Entity, item.ID);
        Source.FireEvent(addToInventory);

        eventData.pointerDrag.GetComponent<DragAndDrop>().Set(InventoryView.position, InventoryView.transform);

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

        Services.WorldUIService.UpdateUI(Source.ID);
    }
}
