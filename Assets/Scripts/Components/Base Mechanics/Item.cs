using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Component
{
    public Item()
    {
        RegisteredEvents.Add(GameEventId.Pickup);
        RegisteredEvents.Add(GameEventId.Drop);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.Pickup)
        {
            IEntity entity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            FireEvent(entity, new GameEvent(GameEventId.AddToInventory, new KeyValuePair<string, object>(EventParameters.Entity, Self.ID)));
        }

        if (gameEvent.ID == GameEventId.Drop)
        {
            IEntity droppingEntity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            FireEvent(World.Instance.Self, new GameEvent(GameEventId.Drop, new KeyValuePair<string, object>(EventParameters.Entity, Self.ID),
                                                                           new KeyValuePair<string, object>(EventParameters.EntityType, EntityType.Item),
                                                                           new KeyValuePair<string, object>(EventParameters.Creature, droppingEntity.ID)));

            FireEvent(droppingEntity, new GameEvent(GameEventId.RemoveFromInventory, new KeyValuePair<string, object>(EventParameters.Entity, Self.ID)));
        }
    }
}

public class DTO_Item : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Item();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Item);
    }
}