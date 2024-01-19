using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class MoveData : EntityComponent
{
    [SerializeField]
    public MovementBlockFlag MovementFlags;
    [SerializeField]
    public Type MonobehaviorType = typeof(Move);

    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.MoveKeyPressed, MoveKeyPressed);
    }

    void MoveKeyPressed(GameEvent gameEvent)
    {
        //m_StopMovement = false;
        MoveDirection direction = (MoveDirection)gameEvent.Paramters[EventParameter.InputDirection];
        Debug.Log(direction.ToString());

        var position = Entity.GetComponent<PositionData>();
        var desiredPosition = Services.Map.GetTilePointInDirection(position.Point, direction);

        var desiredTile = Services.Map.GetTile(desiredPosition);
        if (desiredTile == null)
            return;

        bool canMove = true;
        if ((desiredTile.BlocksMovementFlags & MovementFlags) != 0)
        {
            //Attempt to interact
            var interact = GameEventPool.Get(GameEventId.Interact)
                .With(EventParameter.Source, Entity.GameObject);
            
            Debug.LogWarning("Fix this later, don't just pass in the gameobject");
            desiredTile.FireEvent(Entity.GameObject, interact);
            
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
        Entity.FireEvent(move);
        move.Release();

        if(canMove)
        {
            var afterMoving = GameEventPool.Get(GameEventId.AfterMoving);
            Entity.FireEvent(afterMoving).Release();
        }
    }
}

public class Move : ComponentBehavior<MoveData>
{
    
}