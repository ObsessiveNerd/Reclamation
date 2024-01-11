using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Move : EntityComponent
{
    public MovementBlockFlag MovementFlags;

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.MoveKeyPressed, MoveKeyPressed);
    }

    void MoveKeyPressed(GameEvent gameEvent)
    {
        //m_StopMovement = false;
        MoveDirection direction = (MoveDirection)gameEvent.Paramters[EventParameter.InputDirection];
        Debug.Log(direction.ToString());

        var position = GetComponent<Position>();
        var desiredPosition = Services.Map.GetTilePointInDirection(position.Point, direction);

        var currentTile = Services.Map.GetTile(position.Point);
        var desiredTile = Services.Map.GetTile(desiredPosition);

        bool canMove = true;
        if ((desiredTile.BlocksMovementFlags & MovementFlags) != 0)
        {
            //Attempt to interact
            var interact = GameEventPool.Get(GameEventId.Interact)
                .With(EventParameter.Source, gameObject);
            desiredTile.FireEvent(interact);
            interact.Release();
            canMove = false;
        }

        //var beforeMoving = GameEventPool.Get(GameEventId.BeforeMoving)
        //    .With(EventParameter.TileInSight, desiredPosition)
        //    .With(EventParameter.CanMove, true);
        //gameObject.FireEvent(beforeMoving);

        var move = GameEventPool.Get(GameEventId.MoveEntity)
                .With(EventParameter.TilePosition, desiredPosition)
                .With(EventParameter.CanMove, canMove);
        gameObject.FireEvent(move);
        move.Release();

        if(canMove)
        {
            var afterMoving = GameEventPool.Get(GameEventId.AfterMoving);
            gameObject.FireEvent(afterMoving).Release();
            currentTile.RemoveObject(gameObject);
            desiredTile.AddObject(gameObject);
        }
    }
}