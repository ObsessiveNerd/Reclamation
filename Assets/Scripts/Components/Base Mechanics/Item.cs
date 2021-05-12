using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Component
{
    public Item()
    {
        RegisteredEvents.Add(GameEventId.Pickup);
        RegisteredEvents.Add(GameEventId.Drop);
        RegisteredEvents.Add(GameEventId.GetContextMenuActions);
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

            EventBuilder unequip = new EventBuilder(GameEventId.Unequip)
                                .With(EventParameters.Entity, droppingEntity.ID)
                                .With(EventParameters.Item, Self.ID);

            FireEvent(droppingEntity, unequip.CreateEvent());
            FireEvent(droppingEntity, new GameEvent(GameEventId.RemoveFromInventory, new KeyValuePair<string, object>(EventParameters.Entity, Self.ID)));
        }

        if (gameEvent.ID == GameEventId.GetContextMenuActions)
        {
            IEntity source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            ContextMenuButton button = new ContextMenuButton("Drop", () =>
            {
                EventBuilder drop = new EventBuilder(GameEventId.Drop)
                                        .With(EventParameters.Entity, source.ID);

                FireEvent(Self, drop.CreateEvent(), true);
            });
            gameEvent.GetValue<List<ContextMenuButton>>(EventParameters.InventoryContextActions).Add(button);
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