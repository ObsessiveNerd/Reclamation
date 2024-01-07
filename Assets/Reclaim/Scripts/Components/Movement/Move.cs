using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Move : EntityComponent
{
    public MovementBlockFlag MovementFlags;
    readonly float m_EnergyRequired = 1f;
    bool m_StopMovement = false;

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.MoveKeyPressed, MoveKeyPressed);
        RegisteredEvents.Add(GameEventId.GetMinimumEnergyForAction, GetMinimumEnergyForAction);
        RegisteredEvents.Add(GameEventId.StopMovement, StopMovement);
    }

    void MoveKeyPressed(GameEvent gameEvent)
    {
        //m_StopMovement = false;
        MoveDirection direction = (MoveDirection)gameEvent.Paramters[EventParameter.InputDirection];
        Debug.Log(direction.ToString());

        var position = GetComponent<Position>();
        var desiredPosition = Services.Map.GetTilePointInDirection(position.Point.Value, direction);

        var currentTile = Services.Map.GetTile(position.Point.Value);
        var desiredTile = Services.Map.GetTile(desiredPosition);

        bool canMove = true;
        if ((desiredTile.BlocksMovementFlags & MovementFlags) != 0)
        {
            //Attempt to interact
            var interact = GameEventPool.Get(GameEventId.Interact)
                .With(EventParameter.Source, gameObject);
            desiredTile.FireEvent(interact);
            canMove = false;
            interact.Release();
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
            currentTile.RemoveObject(gameObject);
            desiredTile.AddObject(gameObject);
        }

        //Point startPosition = Services.EntityMapService.GetPointWhereEntityIs(gameObject);
        //if (startPosition == Point.InvalidPoint)
        //    return;

        //GameEvent beforeMoving = GameEventPool.Get(GameEventId.BeforeMoving)
        //        .With(EventParameter.Entity, gameObject)
        //        .With(EventParameter.InputDirection, direction)
        //        .With(EventParameter.RequiredEnergy, m_EnergyRequired);

        //gameObject.FireEvent(beforeMoving);

        //Point desiredTile = Services.TileInteractionService.GetTilePointInDirection(startPosition, direction);

        ////Tile t = Services.TileInteractionService.GetTile(desiredTile);
        ////t.BeforeMoving(beforeMoving);

        //float energyRequired = (float)beforeMoving.Paramters[EventParameter.RequiredEnergy];

        ////Make sure we have enough energy;
        //GameEvent currentEnergyEvent = gameObject.FireEvent(GameEventPool.Get(GameEventId.GetEnergy)
        //        .With(EventParameter.Value, 0.0f));
        //float currentEnergy = currentEnergyEvent.GetValue<float>(EventParameter.Value);

        //if (energyRequired > 0f && currentEnergy >= energyRequired && !m_StopMovement)
        //{
        //    Services.EntityMovementService.Move(gameObject, direction);

        //    //Check if there are any effects that need to occur after moving
        //    GameEvent afterMoving = GameEventPool.Get(GameEventId.AfterMoving).With(beforeMoving.Paramters);
        //    gameObject.FireEvent(afterMoving);

        //    //energyRequired = (float)afterMoving.Paramters[EventParameters.RequiredEnergy];
        //    GameEvent useEnergy = GameEventPool.Get(GameEventId.UseEnergy).With(EventParameter.Value, m_EnergyRequired);
        //    gameObject.FireEvent(useEnergy).Release();

        //    afterMoving.Release();

        //}
        //else if (m_StopMovement)
        //    gameObject.FireEvent(GameEventPool.Get(GameEventId.SkipTurn)).Release();

        //currentEnergyEvent.Release();
        //beforeMoving.Release();
    }

    void GetMinimumEnergyForAction(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameter.Value] = Mathf.Min((float)gameEvent.Paramters[EventParameter.Value] > 0f ? (float)gameEvent.Paramters[EventParameter.Value] : m_EnergyRequired, m_EnergyRequired);
    }

    void StopMovement(GameEvent gameEvent)
    {
        m_StopMovement = true;
    }
}