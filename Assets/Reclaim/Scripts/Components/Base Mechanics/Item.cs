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

public class Item : EntityComponent
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
            GameObject entity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameter.Entity]);
            FireEvent(entity, GameEventPool.Get(GameEventId.AddToInventory)
                .With(EventParameter.Entity, Self.ID), true).Release();
        }

        if (gameEvent.ID == GameEventId.Drop)
        {
            GameObject droppingEntity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameter.Entity]);
            Services.TileInteractionService.Drop(droppingEntity, Self);

            GameEvent unequip = GameEventPool.Get(GameEventId.Unequip)
                                .With(EventParameter.Entity, droppingEntity.ID)
                                .With(EventParameter.Item, Self.ID);

            FireEvent(droppingEntity, unequip, true);
            FireEvent(droppingEntity, GameEventPool.Get(GameEventId.RemoveFromInventory)
                .With(EventParameter.Item, Self.ID), true).Release();
            unequip.Release();

        }

        if (gameEvent.ID == GameEventId.GetContextMenuActions)
        {
            GameObject source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));
            ContextMenuButton dropButton = new ContextMenuButton("Drop", () =>
            {
                GameEvent drop = GameEventPool.Get(GameEventId.Drop)
                                        .With(EventParameter.Entity, source.ID);

                FireEvent(Self, drop, true).Release();
            });
            gameEvent.GetValue<List<ContextMenuButton>>(EventParameter.InventoryContextActions).Add(dropButton);

            ContextMenuButton giveTo = new ContextMenuButton("Give to...", () =>
            {
                Services.WorldUIService.PromptToGiveItem(source,Self);
            });
            gameEvent.GetValue<List<ContextMenuButton>>(EventParameter.InventoryContextActions).Add(giveTo);
        }

        if(gameEvent.ID == GameEventId.GetRarity)
        {
            gameEvent.Paramters[EventParameter.Rarity] = Rarity;
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