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

[Serializable]
public class ItemData : EntityComponent
{
    public ItemRarity Rarity = ItemRarity.Uncommon;

    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.Interact, Pickup);
        RegisteredEvents.Add(GameEventId.Drop, Drop);
        RegisteredEvents.Add(GameEventId.GetContextMenuActions, GetContextMenuActions);
        RegisteredEvents.Add(GameEventId.GetRarity, GetRarity);
    }

    void Pickup(GameEvent gameEvent)
    {
        Entity source = gameEvent.GetValue<GameObject>(EventParameter.Source).GetComponent<EntityBehavior>().Entity;
        var inventory = source.GetComponent<InventoryData>();
        inventory.AddToInventory(Entity);

        //Probably move this to an event
        //PositionData pos = Entity.GetComponent<PositionData>();
        //Services.Map.GetTile(pos.Data.Point).RemoveObject(gameObject);
        //Destroy(gameObject);
    }

    void Drop(GameEvent gameEvent)
    {
        GameObject droppingEntity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameter.Entity]);
        //Services.TileInteractionService.Drop(droppingEntity, gameObject);

        GameEvent unequip = GameEventPool.Get(GameEventId.Unequip)
                                .With(EventParameter.Entity, droppingEntity)
                                .With(EventParameter.Item, Entity.GameObject);

        Entity.FireEvent(unequip);
        Entity.FireEvent(GameEventPool.Get(GameEventId.RemoveFromInventory)
            .With(EventParameter.Item, Entity.GameObject)).Release();
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
        gameEvent.Paramters[EventParameter.Rarity] = Rarity;
    }

}

public class Item : EntityComponentBehavior
{
    public ItemData Data = new ItemData();
    
    public override IComponent GetData()
    {
        return Data;
    }
}