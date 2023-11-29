using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : EntityComponent
{
    readonly float m_EnergyRequired = 1f;
    bool m_StopMovement = false;

    public Move()
    {
        RegisteredEvents.Add(GameEventId.MoveKeyPressed);
        RegisteredEvents.Add(GameEventId.GetMinimumEnergyForAction);
        RegisteredEvents.Add(GameEventId.StopMovement);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.MoveKeyPressed)
        {
            m_StopMovement = false;
            MoveDirection direction;
            if (gameEvent.Paramters[EventParameter.InputDirection] is string)
                direction = (MoveDirection)Enum.Parse(typeof(MoveDirection), gameEvent.Paramters[EventParameter.InputDirection].ToString());
            else
                direction = (MoveDirection)gameEvent.Paramters[EventParameter.InputDirection];

            Point startPosition = Services.EntityMapService.GetPointWhereEntityIs(Self);
            if (startPosition == Point.InvalidPoint)
                return;

            GameEvent beforeMoving = GameEventPool.Get(GameEventId.BeforeMoving)
                .With(EventParameter.Entity, Self.ID)
                .With(EventParameter.InputDirection, direction)
                .With(EventParameter.RequiredEnergy, m_EnergyRequired);

            FireEvent(Self, beforeMoving);

            Point desiredTile = Services.TileInteractionService.GetTilePointInDirection(startPosition, direction);
            Tile t = Services.TileInteractionService.GetTile(desiredTile);

            t.BeforeMoving(beforeMoving);

            float energyRequired = (float)beforeMoving.Paramters[EventParameter.RequiredEnergy];

            //Make sure we have enough energy;
            GameEvent currentEnergyEvent = FireEvent(Self, GameEventPool.Get(GameEventId.GetEnergy)
                .With(EventParameter.Value, 0.0f));
            float currentEnergy = currentEnergyEvent.GetValue<float>(EventParameter.Value);
            
            if (energyRequired > 0f && currentEnergy >= energyRequired && !m_StopMovement)
            {
                Services.EntityMovementService.Move(Self, direction);

                //Check if there are any effects that need to occur after moving
                GameEvent afterMoving = GameEventPool.Get(GameEventId.AfterMoving).With(beforeMoving.Paramters);
                FireEvent(Self, afterMoving);

                //energyRequired = (float)afterMoving.Paramters[EventParameters.RequiredEnergy];
                GameEvent useEnergy = GameEventPool.Get(GameEventId.UseEnergy).With(EventParameter.Value, m_EnergyRequired);
                FireEvent(Self, useEnergy).Release();

                afterMoving.Release();

            }
            else if (m_StopMovement)
                FireEvent(Self, GameEventPool.Get(GameEventId.SkipTurn)).Release();

            currentEnergyEvent.Release();
            beforeMoving.Release();
        }
        if (gameEvent.ID == GameEventId.GetMinimumEnergyForAction)
            gameEvent.Paramters[EventParameter.Value] = Mathf.Min((float)gameEvent.Paramters[EventParameter.Value] > 0f ? (float)gameEvent.Paramters[EventParameter.Value] : m_EnergyRequired, m_EnergyRequired);
        if (gameEvent.ID == GameEventId.StopMovement)
            m_StopMovement = true;
    }
}

public class DTO_Move : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Move();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(Move);
    }
}