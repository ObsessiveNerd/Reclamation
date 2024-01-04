using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : EntityComponent
{
    public List<GameObject> InventoryItems = new List<GameObject>();

    public Inventory()
    {
        RegisteredEvents.Add(GameEventId.OpenInventory, OpenInventory);
        RegisteredEvents.Add(GameEventId.CloseInventory, CloseInventory);
        RegisteredEvents.Add(GameEventId.AddToInventory, AddToInventory);
        RegisteredEvents.Add(GameEventId.RemoveFromInventory, RemoveFromInventory);
        //RegisteredEvents.Add(GameEventId.EmptyBag);
        RegisteredEvents.Add(GameEventId.Died, Died);
        RegisteredEvents.Add(GameEventId.GetCurrentInventory, GetCurrentInventory);
        RegisteredEvents.Add(GameEventId.TryEquip, TryEquip);
    }

    void OpenInventory(GameEvent gameEvent)
    {
        //Services.WorldUIService.OpenInventory();
    }

    void CloseInventory(GameEvent gameEvent)
    {

    }

    public void AddToInventory(GameEvent gameEvent)
    {
        GameObject item = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameter.Entity]);
        if (!InventoryItems.Contains(item))
            InventoryItems.Add(item);
    }

    void RemoveFromInventory(GameEvent gameEvent)
    {
        GameObject item = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameter.Item]);
        if (InventoryItems.Contains(item))
            InventoryItems.Remove(item);
    }

    void Died(GameEvent gameEvent)
    {
        //foreach (GameObject item in InventoryItems)
        //    Services.TileInteractionService.Drop(Self, item);
        //InventoryItems.Clear();
    }

    void TryEquip(GameEvent gameEvent)
    {
        List<GameObject> items = new List<GameObject>(InventoryItems);
        foreach (GameObject item in items)
            FireEvent(item, gameEvent, true);
    }

    void GetCurrentInventory(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameter.Value] = InventoryItems;
    }
}