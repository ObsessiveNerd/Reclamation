using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : EntityComponent
{
    public List<IEntity> InventoryItems = new List<IEntity>();

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

    public void AddToInventory(IEntity e)
    {
        InventoryItems.Add(e);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.OpenInventory)
        {
            Services.WorldUIService.OpenInventory(Self);
        }

        if (gameEvent.ID == GameEventId.AddToInventory)
        {
            IEntity item = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
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
            IEntity item = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Item]);
            if (InventoryItems.Contains(item))
                InventoryItems.Remove(item);
        }

        if (gameEvent.ID == GameEventId.Died)
        {
            foreach (IEntity item in InventoryItems)
                Services.TileInteractionService.Drop(Self, item);
            InventoryItems.Clear();
        }

        if(gameEvent.ID == GameEventId.TryEquip)
        {
            List<IEntity> items = new List<IEntity>(InventoryItems);
            foreach (IEntity item in items)
                FireEvent(item, gameEvent);
        }

        if(gameEvent.ID == GameEventId.GetCurrentInventory)
        {
            gameEvent.Paramters[EventParameters.Value] = InventoryItems;
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
            foreach (IEntity e in EntityFactory.GetEntitiesFromArray(value))
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