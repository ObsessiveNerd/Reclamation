using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : EntityComponent
{
    public List<GameObject> InventoryItems = new List<GameObject>();

    public Inventory()
    {
        //RegisteredEvents.Add(GameEventId.AddToInventory, AddToInventory);
        //RegisteredEvents.Add(GameEventId.RemoveFromInventory, RemoveFromInventory);
        RegisteredEvents.Add(GameEventId.Died, Died);
    }

    public void AddToInventory(GameObject item)
    {
        //GameObject item = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameter.Entity]);
        //if (!InventoryItems.Contains(item))
        
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
}