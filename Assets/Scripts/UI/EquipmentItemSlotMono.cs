using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentItemSlotMono : MonoBehaviour, IDropHandler
{
    public BodyPart Part;
    public GameObject BaseImage;

    InventoryManagerMono inventoryManager;
    IEntity Source;

    public void Setup(IEntity source)
    {
        Source = source;
        //if (transform.childCount > 1)
        //    BaseImage.SetActive(false);
        //else if (transform.childCount == 1)
        //    BaseImage.SetActive(true);
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
                Debug.Log("Source does not equel source");
               //GameEvent unEquip = GameEventPool.Get(GameEventId.Unequip)
               // .With(EventParameters.EntityType, equipmentBodyPart)
               // .With(EventParameters.Item, item.ID)
               // .With(EventParameters.Entity, source.ID);

               // source.FireEvent(unEquip).Release();

                GameEvent removeFromInventory = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                        .With(EventParameters.Item, item.ID);
                source.FireEvent(removeFromInventory).Release();
                removeFromInventory.Release();

                Services.WorldUIService.UpdateUI(source.ID);
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

            if (current.ID == Services.WorldDataQuery.GetActivePlayerId())
            { 
            Debug.Log($"Trying to unequip {item.Name}");

             GameEvent unEquip = GameEventPool.Get(GameEventId.Unequip)
                .With(EventParameters.EntityType, equipmentBodyPart)
                .With(EventParameters.Item, currentItem.ID)
                .With(EventParameters.Entity, current.ID);

            current.FireEvent(unEquip);
            unEquip.Release();
            }
            Destroy(inventoryItem.gameObject);
            //Services.WorldUIService.UpdateUI(current.ID);
        }

        eventData.pointerDrag.GetComponent<DragAndDrop>().Set(transform.position, transform);
        Services.WorldUIService.UpdateUI(Source.ID);
    }
}
