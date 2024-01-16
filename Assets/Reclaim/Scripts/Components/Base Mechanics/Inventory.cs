using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : EntityComponent
{
    public List<ManagedItem> InventoryItems = new List<ManagedItem> ();

    Dictionary<ManagedItem, int> m_ManagedItemCount = new Dictionary<ManagedItem, int> ();

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.GetAmmo, GetAmmo);
        RegisteredEvents.Add(GameEventId.Died, Died);

        //Dictionary<GameObject, int> inventory = new Dictionary <GameObject, int>();
        //foreach (var item in InventoryItems.Keys)
        //    inventory.Add(Services.EntityFactory.Create(item), InventoryItems[item]);
        //InventoryItems = inventory;
    }   

    void GetAmmo(GameEvent gameEvent)
    {
        
    }

    public void AddToInventory(GameObject item)
    {
        //InventoryItems.Add(item, 1);
    }

    public void RemoveFromInventory(GameObject item)
    {
        //InventoryItems[item]--;
        //if (InventoryItems[item] == 0)
        //    InventoryItems.Remove(item);
    }

    //void RemoveFromInventory(GameEvent gameEvent)
    //{
    //    GameObject item = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameter.Item]);
    //    if (InventoryItems.Contains(item))
    //        InventoryItems.Remove(item);
    //}

    void Died(GameEvent gameEvent)
    {
        //foreach (GameObject item in InventoryItems)
        //    Services.TileInteractionService.Drop(Self, item);
        //InventoryItems.Clear();
    }
}