using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drunk : EntityComponent
{
    public int DrunkRounds;
    public int CurrentDrunkRounds;

    public Drunk()
    {
        //Todo: this should be affected by a creatures con score
        DrunkRounds = 12;

        RegisteredEvents.Add(GameEventId.BeforeMoving);
        RegisteredEvents.Add(GameEventId.EndTurn);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.BeforeMoving)
            gameEvent.Paramters[EventParameters.InputDirection] = InputUtility.GetRandomMoveDirection();

        if (gameEvent.ID == GameEventId.EndTurn)
        {
            CurrentDrunkRounds++;
            if(CurrentDrunkRounds >= DrunkRounds)
                Self.RemoveComponent(this);
        }
    }
}

public class DTO_Drunk : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new Drunk();
    }

    public string CreateSerializableData(IComponent component)
    {
        //todo: need to take current and max drunk rounds into account
        return nameof(Drunk);
    }
}
