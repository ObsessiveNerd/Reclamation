using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class EnchantItemSlotMono : MonoBehaviour, IDropHandler
{
    public bool AcceptsDrop;
    public Action<InventoryItemMono> BeforeDrop;
    public Action<InventoryItemMono> Dropped;
    public Action PickedUp;

    public InventoryItemMono ItemMono;

    public void OnDrop(PointerEventData eventData)
    {
        if (AcceptsDrop)
        {
            //Debug.Log($"{eventData.pointerDrag} is dropped");
            //if (ItemMono != null)
            //{
            //    Services.WorldUIService.UpdateUI(Services.WorldDataQuery.GetActivePlayerId());
            //    Destroy(ItemMono.gameObject);
            //    ItemMono = null;
            //}

            BeforeDrop?.Invoke(ItemMono);
            eventData.pointerDrag.GetComponent<DragAndDrop>().Set(transform.position, transform);
            ItemMono = eventData.pointerDrag.GetComponent<InventoryItemMono>();
            ItemMono.AllowConxtMenuOptions = false;
            Dropped?.Invoke(ItemMono);
        }
    }

    //public void Cleanup()
    //{
    //    if (ItemMono == null)
    //        return;

    //    GameEvent removeFromInventory = GameEventPool.Get(GameEventId.RemoveFromInventory)
    //                                    .With(EventParameters.Item, ItemMono.ItemObject.ID);
    //    ItemMono.Source.FireEvent(removeFromInventory);
    //    removeFromInventory.Release();
    //    Destroy(ItemMono.gameObject);
    //}

    bool hadItemPreviously = false;
    public void Update()
    {
        if (transform.childCount == 0)
        {
            //ItemMono = null;
            if (hadItemPreviously && ItemMono != null)
            {
                hadItemPreviously = false;
                PickedUp?.Invoke();
            }
        }
        else
            hadItemPreviously = true;
    }

    public void Cleanup()
    {
        if(ItemMono != null)
            Destroy(ItemMono.gameObject);
    }
}
