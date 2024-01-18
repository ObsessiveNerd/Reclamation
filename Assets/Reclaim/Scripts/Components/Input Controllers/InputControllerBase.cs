using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class InputControllerBase : EntityComponentBehavior
{
    [ServerRpc]
    protected void MoveServerRpc(MoveDirection desiredDirection)
    {
        MoveClientRpc(desiredDirection);
    }

    [ClientRpc]
    protected void MoveClientRpc(MoveDirection desiredDirection)
    {
        gameObject.FireEvent(GameEventPool.Get(GameEventId.MoveKeyPressed)
                        .With(EventParameter.InputDirection, desiredDirection), true).Release();
    }

    [ServerRpc]
    protected void InteractWithTileServerRpc(Point point)
    {
        InteractWithTileClientRpc(point);
    }

    [ClientRpc]
    protected void InteractWithTileClientRpc(Point point)
    {
        Tile currentTile = Services.Map.GetTile(point);
        var interact = GameEventPool.Get(GameEventId.Interact)
                .With(EventParameter.Source, gameObject);
        currentTile.FireEvent(gameObject, interact);
        interact.Release();
    }
}
