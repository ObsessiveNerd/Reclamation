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

public class ItemData : ComponentData
{
    public ItemRarity Rarity = ItemRarity.Uncommon;
}

public class Item : EntityComponent
{
    public ItemData Data = new ItemData();

    public override void WakeUp(IComponentData data = null)
    {
        RegisteredEvents.Add(GameEventId.Interact, Pickup);
        RegisteredEvents.Add(GameEventId.Drop, Drop);
        RegisteredEvents.Add(GameEventId.GetContextMenuActions, GetContextMenuActions);
        RegisteredEvents.Add(GameEventId.GetRarity, GetRarity);

        if (data != null)
            Data = data as ItemData;
    }

    void Pickup(GameEvent gameEvent)
    {
        GameObject source = gameEvent.GetValue<GameObject>(EventParameter.Source);
        var inventory = source.GetComponent<Inventory>();
        inventory.AddToInventory(gameObject);

        Position pos = GetComponent<Position>();
        Services.Map.GetTile(pos.Data.Point).RemoveObject(gameObject);
        Destroy(gameObject);
    }
    public override IComponentData GetData()
    {
        return Data;
    }

    void Drop(GameEvent gameEvent)
    {
        GameObject droppingEntity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameter.Entity]);
        //Services.TileInteractionService.Drop(droppingEntity, gameObject);

        GameEvent unequip = GameEventPool.Get(GameEventId.Unequip)
                                .With(EventParameter.Entity, droppingEntity)
                                .With(EventParameter.Item, gameObject);

        FireEvent(droppingEntity, unequip, true);
        FireEvent(droppingEntity, GameEventPool.Get(GameEventId.RemoveFromInventory)
            .With(EventParameter.Item, gameObject), true).Release();
        unequip.Release();
    }

    void GetContextMenuActions(GameEvent gameEvent)
    {
        GameObject source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));
        ContextMenuButton dropButton = new ContextMenuButton("Drop", () =>
            {
                //GameEvent drop = GameEventPool.Get(GameEventId.Drop)
                //                        .With(EventParameter.Entity, source.ID);

                //FireEvent(Self, drop, true).Release();
            });
        gameEvent.GetValue<List<ContextMenuButton>>(EventParameter.InventoryContextActions).Add(dropButton);

        ContextMenuButton giveTo = new ContextMenuButton("Give to...", () =>
            {
                //Services.WorldUIService.PromptToGiveItem(source,Self);
            });
        gameEvent.GetValue<List<ContextMenuButton>>(EventParameter.InventoryContextActions).Add(giveTo);
    }

    void GetRarity(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameter.Rarity] = Data.Rarity;
    }
}