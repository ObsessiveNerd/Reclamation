using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class InputControllerBase : ComponentBehavior<EntityComponent>
{
    protected Entity entity;
    
    protected virtual void Start()
    {
        entity = GetComponent<EntityBehavior>().Entity;
    }

    [ServerRpc]
    protected void MoveServerRpc(MoveDirection desiredDirection)
    {
        if (entity == null || !entity.IsActive)
            return;

        MoveClientRpc(desiredDirection);
    }

    [ClientRpc]
    protected void MoveClientRpc(MoveDirection desiredDirection)
    {
        if (entity == null || !entity.IsActive)
            return;

        gameObject.FireEvent(GameEventPool.Get(GameEventId.MoveKeyPressed)
                        .With(EventParameter.InputDirection, desiredDirection), true).Release();
    }

    [ServerRpc]
    protected void InteractWithTileServerRpc(Point point)
    {
        if (entity == null || !entity.IsActive)
            return;

        InteractWithTileClientRpc(point);
    }

    [ClientRpc]
    protected void InteractWithTileClientRpc(Point point)
    {
        if (entity == null || !entity.IsActive)
            return;

        Tile currentTile = Services.Map.GetTile(point);
        var interact = GameEventPool.Get(GameEventId.Interact)
                .With(EventParameter.Source, gameObject);
        currentTile.FireEvent(gameObject, interact);
        interact.Release();
    }

    [ServerRpc]
    protected void PrimaryActionServerRpc(Point point)
    {
        PrimaryActionClientRpc(point);
    }

    [ClientRpc]
    protected void PrimaryActionClientRpc(Point point)
    {
        Tile t = Services.Map.GetTile(point);
        if (t == null)
        {
            Debug.LogError("NULL TILE BRUH");
            return;
        }

        var primaryAction = GameEventPool.Get(GameEventId.PrimaryInteraction)
                        .With(EventParameter.Source, gameObject);
        t.FireEvent(gameObject, primaryAction);
        primaryAction.Release();
    }

    [ServerRpc]
    protected void SecondaryActionServerRpc(Point point)
    {
        SecondaryActionClientRpc(point);
    }

    [ClientRpc]
    protected void SecondaryActionClientRpc(Point point)
    {
        var e = GetComponent<Inventory>().component.InventoryEntities[0];
        var drop = GameEventPool.Get(GameEventId.Drop)
                .With(EventParameter.Source, GetComponent<EntityBehavior>().Entity);
        e.FireEvent(drop);
        drop.Release();
    }
}
