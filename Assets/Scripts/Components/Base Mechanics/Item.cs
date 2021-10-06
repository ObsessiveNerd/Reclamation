using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemRarity
{
    Common = 1,
    Uncommon = 3,
    Rare = 5,
    Epic = 10,
    Mythic = 20
}

public class Item : Component
{
    public ItemRarity Rarity;

    public Item(ItemRarity rarity)
    {
        Rarity = rarity;

        RegisteredEvents.Add(GameEventId.Pickup);
        RegisteredEvents.Add(GameEventId.Drop);
        RegisteredEvents.Add(GameEventId.GetContextMenuActions);
        RegisteredEvents.Add(GameEventId.GetRarity);
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

            EventBuilder unequip = EventBuilderPool.Get(GameEventId.Unequip)
                                .With(EventParameters.Entity, droppingEntity.ID)
                                .With(EventParameters.Item, Self.ID);

            FireEvent(droppingEntity, unequip.CreateEvent());
            FireEvent(droppingEntity, new GameEvent(GameEventId.RemoveFromInventory, new KeyValuePair<string, object>(EventParameters.Entity, Self.ID)));
        }

        if (gameEvent.ID == GameEventId.GetContextMenuActions)
        {
            IEntity source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            ContextMenuButton dropButton = new ContextMenuButton("Drop", () =>
            {
                EventBuilder drop = EventBuilderPool.Get(GameEventId.Drop)
                                        .With(EventParameters.Entity, source.ID);

                FireEvent(Self, drop.CreateEvent(), true);
            });
            gameEvent.GetValue<List<ContextMenuButton>>(EventParameters.InventoryContextActions).Add(dropButton);

            ContextMenuButton giveTo = new ContextMenuButton("Give to...", () =>
            {
                EventBuilder giveItemTo = EventBuilderPool.Get(GameEventId.PromptToGiveItem)
                                             .With(EventParameters.Entity, gameEvent.GetValue<string>(EventParameters.Entity))
                                             .With(EventParameters.Item, Self.ID);
                World.Instance.Self.FireEvent(giveItemTo.CreateEvent());
            });
            gameEvent.GetValue<List<ContextMenuButton>>(EventParameters.InventoryContextActions).Add(giveTo);
        }

        if(gameEvent.ID == GameEventId.GetRarity)
        {
            gameEvent.Paramters[EventParameters.Rarity] = Rarity;
        }
    }
}

public class DTO_Item : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string value = data;
        if (value.Contains("="))
        {
            value = data.Split('=')[1];
            Component = new Item((ItemRarity)Enum.Parse(typeof(ItemRarity), value));
        }
        else
            Component = new Item(ItemRarity.Common);
    }

    public string CreateSerializableData(IComponent component)
    {
        Item item = (Item)component;
        return $"{nameof(Item)}:{nameof(item.Rarity)}={item.Rarity}";
    }
}