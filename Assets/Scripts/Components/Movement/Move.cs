using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Component
{
    float m_EnergyRequired = 1f;
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
            if (gameEvent.Paramters[EventParameters.InputDirection] is string)
                direction = (MoveDirection)Enum.Parse(typeof(MoveDirection), gameEvent.Paramters[EventParameters.InputDirection].ToString());
            else
                direction = (MoveDirection)gameEvent.Paramters[EventParameters.InputDirection];

            GameEvent beforeMoving = GameEventPool.Get(GameEventId.BeforeMoving)
                .With(EventParameters.InputDirection, direction)
                .With(EventParameters.RequiredEnergy, m_EnergyRequired);

            FireEvent(Self, beforeMoving);

            //Need to get a result from this and check that the world says it's valid to move in the desired direction
            GameEvent beforeMovingCheckWorld = GameEventPool.Get(GameEventId.BeforeMoving)
                .With(EventParameters.Entity, Self.ID)
                .With(EventParameters.EntityType, EntityType.Creature)
                .With(EventParameters.InputDirection, beforeMoving.Paramters[EventParameters.InputDirection])
                .With(EventParameters.RequiredEnergy, m_EnergyRequired);
            
            FireEvent(World.Instance.Self, beforeMovingCheckWorld);

            float energyRequired = (float)beforeMovingCheckWorld.Paramters[EventParameters.RequiredEnergy];

            //Make sure we have enough energy;
            GameEvent currentEnergyEvent = FireEvent(Self, GameEventPool.Get(GameEventId.GetEnergy)
                .With(EventParameters.Value, 0.0f));
            float currentEnergy = currentEnergyEvent.GetValue<float>(EventParameters.Value);
            currentEnergyEvent.Release();
            
            if (energyRequired > 0f && currentEnergy >= energyRequired && !m_StopMovement)
            {
                GameEvent moveWorld = GameEventPool.Get(GameEventId.MoveEntity).With(EventParameters.Entity, Self.ID)
                                                                                .With(EventParameters.EntityType, EntityType.Creature)
                                                                                .With(EventParameters.InputDirection, beforeMovingCheckWorld.Paramters[EventParameters.InputDirection])
                                                                                .With(EventParameters.RequiredEnergy, energyRequired);
                FireEvent(World.Instance.Self, moveWorld);

                //See if there's anything on the player that needs to happen when moving
                GameEvent moving = GameEventPool.Get(GameEventId.ExecuteMove).With(moveWorld.Paramters);
                FireEvent(Self, moving);


                //Check if there are any effects that need to occur after moving
                GameEvent afterMoving = GameEventPool.Get(GameEventId.AfterMoving).With(moving.Paramters);
                FireEvent(Self, afterMoving).Release();

                //energyRequired = (float)afterMoving.Paramters[EventParameters.RequiredEnergy];
                GameEvent useEnergy = GameEventPool.Get(GameEventId.UseEnergy).With(EventParameters.Value, energyRequired);
                FireEvent(Self, useEnergy).Release();

                moving.Release();
                moveWorld.Release();
                beforeMovingCheckWorld.Release();
                beforeMoving.Release();

            }
            else if (m_StopMovement)
                FireEvent(Self, GameEventPool.Get(GameEventId.SkipTurn)).Release();
        }
        if (gameEvent.ID == GameEventId.GetMinimumEnergyForAction)
            gameEvent.Paramters[EventParameters.Value] = Mathf.Min((float)gameEvent.Paramters[EventParameters.Value] > 0f ? (float)gameEvent.Paramters[EventParameters.Value] : m_EnergyRequired, m_EnergyRequired);
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