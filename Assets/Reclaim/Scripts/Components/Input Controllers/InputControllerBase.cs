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
    protected void MoveServerRpc(MoveDirection desiredDirection, float inputX, float inputY)
    {
        if (entity == null || !entity.IsActive)
            return;

        MoveClientRpc(desiredDirection, inputX, inputY);
    }

    [ClientRpc]
    protected void MoveClientRpc(MoveDirection desiredDirection, float inputX, float inputY)
    {
        if (entity == null || !entity.IsActive)
            return;

        gameObject.FireEvent(GameEventPool.Get(GameEventId.MoveKeyPressed)
                        .With(EventParameter.InputDirection, desiredDirection)
                        .With(EventParameter.InputX, inputX)
                        .With(EventParameter.InputY, inputY)).Release();
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
        Tile t = Services.Map.GetTile(point);
        if (t == null)
        {
            Debug.LogError("NULL TILE BRUH");
            return;
        }

        var secondaryAction = GameEventPool.Get(GameEventId.SecondaryInteraction)
                        .With(EventParameter.Source, gameObject);
        t.FireEvent(gameObject, secondaryAction);
        secondaryAction.Release();
    }
}
