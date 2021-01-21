using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Component
{
    float m_EnergyRequired = 1f;

    public Move()
    {
        RegisteredEvents.Add(GameEventId.MoveKeyPressed);
        RegisteredEvents.Add(GameEventId.GetMinimumEnergyForAction);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.MoveKeyPressed)
        {
            MoveDirection direction = (MoveDirection)gameEvent.Paramters[EventParameters.InputDirection];
            KeyValuePair<string, object> data = new KeyValuePair<string, object>(EventParameters.InputDirection, direction);
            KeyValuePair<string, object> requiredEnergy = new KeyValuePair<string, object>(EventParameters.RequiredEnergy, m_EnergyRequired);
            GameEvent beforeMoving = new GameEvent(GameEventId.BeforeMoving, data, requiredEnergy);
            FireEvent(Self, beforeMoving);

            //Need to get a result from this and check that the world says it's valid to move in the desired direction
            GameEvent beforeMovingCheckWorld = new GameEvent(GameEventId.BeforeMoving, new KeyValuePair<string, object>(EventParameters.Entity, Self),
                                                                            new KeyValuePair<string, object>(EventParameters.EntityType, EntityType.Creature),
                                                                            new KeyValuePair<string, object>(EventParameters.InputDirection, beforeMoving.Paramters[EventParameters.InputDirection]),
                                                                            requiredEnergy);
            FireEvent(World.Instance.Self, beforeMovingCheckWorld);

            float energyRequired = (float)beforeMovingCheckWorld.Paramters[EventParameters.RequiredEnergy];

            //Make sure we have enough energy;
            float currentEnergy = (float)FireEvent(Self, new GameEvent(GameEventId.GetEnergy, new KeyValuePair<string, object>(EventParameters.Value, 0))).Paramters[EventParameters.Value];
            if (energyRequired > 0f && currentEnergy >= energyRequired)
            {
                //See if there's anything on the player that needs to happen when moving
                GameEvent moving = new GameEvent(GameEventId.ExecuteMove, beforeMovingCheckWorld.Paramters);
                FireEvent(Self, moving);

                GameEvent moveWorld = new GameEvent(GameEventId.MoveEntity, new KeyValuePair<string, object>(EventParameters.Entity, Self),
                                                                                new KeyValuePair<string, object>(EventParameters.EntityType, EntityType.Creature),
                                                                                new KeyValuePair<string, object>(EventParameters.InputDirection, moving.Paramters[EventParameters.InputDirection]),
                                                                                requiredEnergy);
                FireEvent(World.Instance.Self, moveWorld);

                //Check if there are any effects that need to occur after moving
                GameEvent afterMoving = new GameEvent(GameEventId.AfterMoving, moving.Paramters);
                FireEvent(Self, afterMoving);

                //energyRequired = (float)afterMoving.Paramters[EventParameters.RequiredEnergy];
                GameEvent useEnergy = new GameEvent(GameEventId.UseEnergy, new KeyValuePair<string, object>(EventParameters.Value, energyRequired));
                FireEvent(Self, useEnergy);
            }
        }
        if (gameEvent.ID == GameEventId.GetMinimumEnergyForAction)
            gameEvent.Paramters[EventParameters.Value] = Mathf.Min((float)gameEvent.Paramters[EventParameters.Value] > 0f ? (float)gameEvent.Paramters[EventParameters.Value] : m_EnergyRequired, m_EnergyRequired);
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