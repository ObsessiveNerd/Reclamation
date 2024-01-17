using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryData : ComponentData
{

}

public class Inventory : EntityComponent
{
    public InventoryData Data = new InventoryData();

    public List<GameObject> InventoryItems = new List<GameObject> ();

    //Dictionary<ManagedItem, int> m_ManagedItemCount = new Dictionary<ManagedItem, int> ();

    public override void WakeUp(IComponentData data = null)
    {
        RegisteredEvents.Add(GameEventId.GetAmmo, GetAmmo);
        RegisteredEvents.Add(GameEventId.Died, Died);

        if (data != null)
            Data = data as InventoryData;
    }

    public override IComponentData GetData()
    {
        return Data;
    }

    void GetAmmo(GameEvent gameEvent)
    {
        
    }

    public void AddToInventory(GameObject item)
    {
        InventoryItems.Add(item);

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