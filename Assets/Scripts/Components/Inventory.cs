using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Component
{
    List<IEntity> m_Inventory = new List<IEntity>();
    bool m_EmptyBag = false;

    public Inventory(IEntity self)
    {
        Init(self);

        RegisteredEvents.Add(GameEventId.OpenInventory);
        RegisteredEvents.Add(GameEventId.CloseInventory);
        RegisteredEvents.Add(GameEventId.AddToInventory);
        RegisteredEvents.Add(GameEventId.RemoveFromInventory);
        RegisteredEvents.Add(GameEventId.EmptyBag);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.OpenInventory)
        {
            foreach (var i in m_Inventory)
                Debug.Log(i);
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
                FireEvent(item, new GameEvent(GameEventId.Drop, new KeyValuePair<string, object>(EventParameters.Entity, Self)));
            m_EmptyBag = false;
            m_Inventory.Clear();
        }
    }
}
