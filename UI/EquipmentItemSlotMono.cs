using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentItemSlotMono : MonoBehaviour, IDropHandler
{
    public BodyPart Part;
    public GameObject BaseImage;

    InventoryManagerMono inventoryManager;
    GameObject Source;

    public void Setup(GameObject source)
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
        GameObject source = eventData.pointerDrag.GetComponent<InventoryItemMono>().Source;
        GameObject item = eventData.pointerDrag.GetComponent<InventoryItemMono>().ItemObject;

        GameEvent getBodyPartForEquipment = GameEventPool.Get(GameEventId.GetBodyPartType)
            .With(EventParameter.BodyPart, BodyPart.None);
        item.FireEvent(getBodyPartForEquipment);
        BodyPart equipmentBodyPart = getBodyPartForEquipment.GetValue<BodyPart>(EventParameter.BodyPart);
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
                                        .With(EventParameter.Item, item.ID);
                source.FireEvent(removeFromInventory).Release();
                removeFromInventory.Release();

                Services.WorldUIService.UpdateUI();
            }

            GameEvent equip = GameEventPool.Get(GameEventId.Equip)
                .With(EventParameter.EntityType, equipmentBodyPart)
                .With(EventParameter.Equipment, item.ID);

            Source.FireEvent(equip);
            equip.Release();
        }

        foreach(var inventoryItem in transform.GetComponentsInChildren<InventoryItemMono>())
        {
            GameObject current = inventoryItem.Source;
            GameObject currentItem = inventoryItem.ItemObject;

            if (current.ID == Services.WorldDataQuery.GetActivePlayer())
            { 
            Debug.Log($"Trying to unequip {item.Name}");

             GameEvent unEquip = GameEventPool.Get(GameEventId.Unequip)
                .With(EventParameter.EntityType, equipmentBodyPart)
                .With(EventParameter.Item, currentItem.ID)
                .With(EventParameter.Entity, current.ID);

            current.FireEvent(unEquip);
            unEquip.Release();
            }
            Destroy(inventoryItem.gameObject);
            //Services.WorldUIService.UpdateUI(current.ID);
        }

        eventData.pointerDrag.GetComponent<DragAndDrop>().Set(transform.position, transform);
        Services.WorldUIService.UpdateUI();
    }
}
