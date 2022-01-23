using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentItemSlotMono : MonoBehaviour, IDropHandler
{
    public BodyPart Part;
    InventoryManagerMono inventoryManager;
    IEntity Source;

    public void Setup(IEntity source)
    {
        Source = source;
    }

    void Start()
    {
        inventoryManager = transform.parent.parent.GetComponentInChildren<InventoryManagerMono>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped");
        IEntity source = eventData.pointerDrag.GetComponent<InventoryItemMono>().Source;
        IEntity item = eventData.pointerDrag.GetComponent<InventoryItemMono>().ItemObject;

        GameEvent getBodyPartForEquipment = GameEventPool.Get(GameEventId.GetBodyPartType)
            .With(EventParameters.BodyPart, BodyPart.None);
        item.FireEvent(getBodyPartForEquipment);
        BodyPart equipmentBodyPart = getBodyPartForEquipment.GetValue<BodyPart>(EventParameters.BodyPart);
        getBodyPartForEquipment.Release();

        if (equipmentBodyPart == Part)
        {
            if(source != Source)
            {
               GameEvent unEquip = GameEventPool.Get(GameEventId.Unequip)
                .With(EventParameters.EntityType, equipmentBodyPart)
                .With(EventParameters.Item, item.ID)
                .With(EventParameters.Entity, source.ID);

                source.FireEvent(unEquip).Release();

                GameEvent removeFromInventory = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                        .With(EventParameters.Entity, item.ID);
                source.FireEvent(removeFromInventory).Release();
            }

            GameEvent equip = GameEventPool.Get(GameEventId.Equip)
                .With(EventParameters.EntityType, equipmentBodyPart)
                .With(EventParameters.Equipment, item.ID);

            Source.FireEvent(equip);
            equip.Release();
        }

        foreach(var inventoryItem in transform.GetComponentsInChildren<InventoryItemMono>())
        {
            IEntity current = inventoryItem.Source;
            IEntity currentItem = inventoryItem.ItemObject;

             GameEvent unEquip = GameEventPool.Get(GameEventId.Unequip)
                .With(EventParameters.EntityType, equipmentBodyPart)
                .With(EventParameters.Item, currentItem.ID);

            current.FireEvent(unEquip);
            unEquip.Release();

            inventoryItem.Set(inventoryManager.InventoryView.position, inventoryManager.InventoryView);
            Services.WorldUIService.UpdateUI(current.ID);
        }

        eventData.pointerDrag.GetComponent<DragAndDrop>().Set(transform.position, transform);
        Services.WorldUIService.UpdateUI(Source.ID);
    }
}
