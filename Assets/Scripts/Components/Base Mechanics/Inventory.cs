using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Component
{
    public List<IEntity> InventoryItems = new List<IEntity>();

    public Inventory()
    {
        RegisteredEvents.Add(GameEventId.OpenInventory);
        RegisteredEvents.Add(GameEventId.CloseInventory);
        RegisteredEvents.Add(GameEventId.AddToInventory);
        RegisteredEvents.Add(GameEventId.RemoveFromInventory);
        RegisteredEvents.Add(GameEventId.EmptyBag);
    }

    //void EntityDestroyed(IEntity e)
    //{
    //    if (InventoryItems.Contains(e))
    //    {
    //        e.Destroyed -= EntityDestroyed;
    //        InventoryItems.Remove(e);
    //    }
    //}

    public void AddToInventory(IEntity e)
    {
        InventoryItems.Add(e);
        //e.Destroyed += EntityDestroyed;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.OpenInventory)
        {
            FireEvent(World.Instance.Self, new GameEvent(GameEventId.OpenInventoryUI, new KeyValuePair<string, object>(EventParameters.Value, InventoryItems),
                                                                                        new KeyValuePair<string, object>(EventParameters.Entity, Self.ID)));
        }

        if(gameEvent.ID == GameEventId.AddToInventory)
        {
            IEntity item = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            InventoryItems.Add(item);
        }

        if(gameEvent.ID == GameEventId.RemoveFromInventory)
        {
            IEntity item = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            if (InventoryItems.Contains(item))
            {
                //item.Destroyed -= EntityDestroyed;
                InventoryItems.Remove(item);
            }
        }

        //if(gameEvent.ID == GameEventId.EmptyBag)
        //{
        //    foreach (IEntity item in InventoryItems)
        //        FireEvent(World.Instance.Self, new GameEvent(GameEventId.Drop, new KeyValuePair<string, object>(EventParameters.Entity, item),
        //                                                                        new KeyValuePair<string, object>(EventParameters.Creature, Self),
        //                                                                        new KeyValuePair<string, object>(EventParameters.EntityType, EntityType.Item)));
        //    InventoryItems.Clear();
        //}
    }
}


public class DTO_Inventory : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Inventory();
        if(!string.IsNullOrEmpty(data))
        {
            foreach (IEntity e in EntityFactory.GetEntitiesFromArray(data))
                ((Inventory)Component).AddToInventory(e);

        }
    }

    public string CreateSerializableData(IComponent component)
    {
        Inventory inventory = (Inventory)component;
        foreach(var item in inventory.InventoryItems)
            EntityFactory.CreateTemporaryBlueprint($"{World.Instance.Seed}", item.ID, item.Serialize()); //todo: feed proper seed

        return $"{nameof(Inventory)}: [{EntityFactory.ConvertEntitiesToStringArray(inventory.InventoryItems)}]";
    }
}