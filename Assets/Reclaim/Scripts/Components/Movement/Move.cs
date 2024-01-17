using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MoveData : IComponentData
{
    public MovementBlockFlag MovementFlags;
}

public class Move : EntityComponent
{
    public MoveData Data = new MoveData();

    public override void WakeUp(IComponentData data = null)
    {
        RegisteredEvents.Add(GameEventId.MoveKeyPressed, MoveKeyPressed);
        if(data != null)
            Data = data as MoveData;
    }

    void MoveKeyPressed(GameEvent gameEvent)
    {
        //m_StopMovement = false;
        MoveDirection direction = (MoveDirection)gameEvent.Paramters[EventParameter.InputDirection];
        Debug.Log(direction.ToString());

        var position = GetComponent<Position>();
        var desiredPosition = Services.Map.GetTilePointInDirection(position.Data.Point, direction);

        var desiredTile = Services.Map.GetTile(desiredPosition);

        bool canMove = true;
        if ((desiredTile.BlocksMovementFlags & Data.MovementFlags) != 0)
        {
            //Attempt to interact
            var interact = GameEventPool.Get(GameEventId.Interact)
                .With(EventParameter.Source, gameObject);
            desiredTile.FireEvent(gameObject, interact);
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
        }
    }
}