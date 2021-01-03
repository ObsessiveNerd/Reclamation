using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : Component
{
    public AIController(IEntity self)
    {
        Init(self);
        RegisteredEvents.Add(GameEventId.UpdateEntity);
        RegisteredEvents.Add(GameEventId.HasInputController);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            MoveDirection desiredDirection = MoveDirection.W; //obviously temp
            desiredDirection = (MoveDirection)FireEvent(Self, 
                new GameEvent(GameEventId.MoveKeyPressed, new KeyValuePair<string, object>(EventParameters.InputDirection, desiredDirection)))
                .Paramters[EventParameters.InputDirection];

            if (desiredDirection == MoveDirection.None)
                FireEvent(Self, new GameEvent(GameEventId.SkipTurn));

            GameEvent checkForEnergy = new GameEvent(GameEventId.HasEnoughEnergyToTakeATurn, new KeyValuePair<string, object>(EventParameters.TakeTurn, false));
            FireEvent(Self, checkForEnergy);
            gameEvent.Paramters[EventParameters.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameters.TakeTurn];
        }

        if (gameEvent.ID == GameEventId.HasInputController)
            gameEvent.Paramters[EventParameters.Value] = true;
    }
}
