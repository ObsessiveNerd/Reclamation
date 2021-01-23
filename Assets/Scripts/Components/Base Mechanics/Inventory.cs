using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Component
{
    List<IEntity> m_Inventory = new List<IEntity>();
    bool m_EmptyBag = false;

    public Inventory()
    {
        RegisteredEvents.Add(GameEventId.OpenInventory);
        RegisteredEvents.Add(GameEventId.CloseInventory);
        RegisteredEvents.Add(GameEventId.AddToInventory);
        RegisteredEvents.Add(GameEventId.RemoveFromInventory);
        RegisteredEvents.Add(GameEventId.EmptyBag);
    }

    public void AddToInventory(IEntity e)
    {
        m_Inventory.Add(e);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.OpenInventory)
        {
            FireEvent(World.Instance.Self, new GameEvent(GameEventId.OpenInventoryUI, new KeyValuePair<string, object>(EventParameters.Value, m_Inventory),
                                                                                        new KeyValuePair<string, object>(EventParameters.Entity, Self)));
        }

        if(gameEvent.ID == GameEventId.AddToInventory)
        {
            IEntity item = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            m_Inventory.Add(item);
        }

        if(gameEvent.ID == GameEventId.RemoveFromInventory)
        {
            IEntity item = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            if (!m_EmptyBag)
                m_Inventory.Remove(item);
        }

        if(gameEvent.ID == GameEventId.EmptyBag)
        {
            m_EmptyBag = true;
            foreach (IEntity item in m_Inventory)
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.Drop, new KeyValuePair<string, object>(EventParameters.Entity, item),
                                                                                new KeyValuePair<string, object>(EventParameters.Creature, Self),
                                                                                new KeyValuePair<string, object>(EventParameters.EntityType, EntityType.Item)));
            m_EmptyBag = false;
            m_Inventory.Clear();
        }
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
        //Todo: need to collect everything in the inventory first
        return nameof(Inventory);
    }
}