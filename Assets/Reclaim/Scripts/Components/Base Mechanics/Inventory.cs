using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryData : EntityComponent
{
    [SerializeField]
    List<GameObject> InventoryItems = new List<GameObject> ();

    public List<Entity> InventoryEntities = new List<Entity>();

    Type MonobehaviorType = typeof(InventoryData);

    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.GetAmmo, GetAmmo);
        RegisteredEvents.Add(GameEventId.Died, Died);
        RegisteredEvents.Add(GameEventId.RemoveFromInventory, RemoveFromInventory);
    }
    void GetAmmo(GameEvent gameEvent)
    {
        
    }

    public void AddToInventory(Entity item)
    {
        InventoryEntities.Add(item);
    }
    
    public void AddToInventory(GameObject item)
    {
        InventoryItems.Add(item);
        InventoryEntities.Add(item.GetComponent<Entity>());

        //InventoryItems.Add(item, 1);
    }

    public void RemoveFromInventory(GameEvent gameEvent) 
    {
        var item = gameEvent.GetValue<Entity>(EventParameter.Item);
        InventoryEntities.Remove(item);
    }

    public void RemoveFromInventory(GameObject item)
    {
        //InventoryItems[item]--;
        //if (InventoryItems[item] == 0)
        //    InventoryItems.Remove(item);
    }

    void Died(GameEvent gameEvent)
    {
        //foreach (GameObject item in InventoryItems)
        //    Services.TileInteractionService.Drop(Self, item);
        //InventoryItems.Clear();
    }
}

public class Inventory : ComponentBehavior<InventoryData>
{
    
}