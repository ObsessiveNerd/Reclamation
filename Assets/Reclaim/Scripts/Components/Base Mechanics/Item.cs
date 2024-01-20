using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
    [SerializeField]
    public ItemRarity Rarity = ItemRarity.Uncommon;

    public Action OnPickup;
    public Action OnDrop;

    [SerializeField]
    public Type MonobehaviorType = typeof(Item);

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
        
        if(OnPickup != null)
            OnPickup();
    }

    void Drop(GameEvent gameEvent)
    {
        Entity source = gameEvent.GetValue<Entity>(EventParameter.Source);

        //GameEvent unequip = GameEventPool.Get(GameEventId.Unequip)
        //                        .With(EventParameter.Entity, souce)
        //                        .With(EventParameter.Item, Entity.GameObject);

        //Entity.FireEvent(unequip);
        //unequip.Release();

        source.FireEvent(GameEventPool.Get(GameEventId.RemoveFromInventory)
            .With(EventParameter.Item, Entity)).Release();

        if(OnDrop != null) 
            OnDrop();
    }

    void GetContextMenuActions(GameEvent gameEvent)
    {
        //GameObject source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));
        //ContextMenuButton dropButton = new ContextMenuButton("Drop", () =
        //    {
        //        //GameEvent drop = GameEventPool.Get(GameEventId.Drop)
        //        //                        .With(EventParameter.Entity, source.ID);

        //        //FireEvent(Self, drop, true).Release();
        //    });
        //gameEvent.GetValue<List<ContextMenuButton>>(EventParameter.InventoryContextActions).Add(dropButton);

        //ContextMenuButton giveTo = new ContextMenuButton("Give to...", () =
        //    {
        //        //Services.WorldUIService.PromptToGiveItem(source,Self);
        //    });
        //gameEvent.GetValue<List<ContextMenuButton>>(EventParameter.InventoryContextActions).Add(giveTo);
    }

    void GetRarity(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameter.Rarity] = Rarity;
    }

}

public class Item : ComponentBehavior<ItemData>
{
    void Start()
    {
        component.OnPickup += PickedUp;
        component.OnDrop += Dropped;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        component.OnPickup -= PickedUp;
    }

    void Dropped()
    {
        Point p =  component.Entity.GetComponent<PositionData>().Point;
        Services.Spawner.SpawnGameObject(component.Entity, p);
    }

    void PickedUp()
    {
        PositionData pos = gameObject.GetComponent<Position>().component;
        Services.Map.GetTile(pos.Point).RemoveObject(gameObject);
        Services.Spawner.DestroyGameObjectServerRpc(gameObject.GetComponent<NetworkObject>().NetworkObjectId);
    }
}