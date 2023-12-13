using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : EntityComponent
{
    public List<GameObject> InventoryItems = new List<GameObject>();

    public Inventory()
    {
        RegisteredEvents.Add(GameEventId.OpenInventory);
        RegisteredEvents.Add(GameEventId.CloseInventory);
        RegisteredEvents.Add(GameEventId.AddToInventory);
        RegisteredEvents.Add(GameEventId.RemoveFromInventory);
        RegisteredEvents.Add(GameEventId.EmptyBag);
        RegisteredEvents.Add(GameEventId.Died);
        RegisteredEvents.Add(GameEventId.GetCurrentInventory);
        RegisteredEvents.Add(GameEventId.TryEquip);
    }

    public void AddToInventory(GameObject e)
    {
        InventoryItems.Add(e);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.OpenInventory)
        {
            Services.WorldUIService.OpenInventory();
        }

        if (gameEvent.ID == GameEventId.AddToInventory)
        {
            GameObject item = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameter.Entity]);
            if (!InventoryItems.Contains(item))
            {
                InventoryItems.Add(item);
                //if (WorldUtility.IsActivePlayer(Self.ID))
                //    Services.WorldUIService.UpdateUI(Self.ID);
                    //FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.UpdateUI)
                    //    .With(EventParameters.Entity, Self.ID)).Release();
            }
        }

        if (gameEvent.ID == GameEventId.RemoveFromInventory)
        {
            GameObject item = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameter.Item]);
            if (InventoryItems.Contains(item))
                InventoryItems.Remove(item);
        }

        if (gameEvent.ID == GameEventId.Died)
        {
            foreach (GameObject item in InventoryItems)
                Services.TileInteractionService.Drop(Self, item);
            InventoryItems.Clear();
        }

        if(gameEvent.ID == GameEventId.TryEquip)
        {
            List<GameObject> items = new List<GameObject>(InventoryItems);
            foreach (GameObject item in items)
                FireEvent(item, gameEvent, true);
        }

        if(gameEvent.ID == GameEventId.GetCurrentInventory)
        {
            gameEvent.Paramters[EventParameter.Value] = InventoryItems;
        }
    }
}


public class DTO_Inventory : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string value = data;
        if (data.Contains("="))
            value = data.Split('=')[1];

        Component = new Inventory();
        if(!string.IsNullOrEmpty(value))
        {
            foreach (GameObject e in EntityFactory.GetEntitiesFromArray(value))
                ((Inventory)Component).AddToInventory(e);

        }
    }

    public string CreateSerializableData(IComponent component)
    {
        Inventory inventory = (Inventory)component;
        foreach(var item in inventory.InventoryItems)
            if(item != null)
                EntityFactory.CreateTemporaryBlueprint(item.ID, item.Serialize()); //todo: feed proper seed

        return $"{nameof(Inventory)}: [{EntityFactory.ConvertEntitiesToStringArrayWithId(inventory.InventoryItems)}]";
    }
}