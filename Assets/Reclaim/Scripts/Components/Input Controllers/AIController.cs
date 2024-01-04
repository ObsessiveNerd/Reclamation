using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AIAction
{
    public int Priority;
    public Func<MoveDirection> ActionToTake;
}

public class AIActionPriorityComparer : IComparer<AIAction>
{
    public int Compare(AIAction x, AIAction y)
    {
        if (x.Priority < y.Priority)
            return -1;
        if (x.Priority == y.Priority)
            return 0;
        return 1;
    }
}

public class AIController : InputControllerBase
{
    private void Start()
    {
        RegisteredEvents.Add(GameEventId.UpdateEntity, UpdateEntity);
    }

    void UpdateEntity(GameEvent gameEvent)
    {
        using (new DiagnosticsTimer("AI Controller"))
        {
            MoveDirection desiredDirection = MoveDirection.None; //InputUtility.GetRandomMoveDirection(); //obviously temp

            GameEvent getActionEventBuilder = GameEventPool.Get(GameEventId.GetActionToTake)
                                                        .With(EventParameter.AIActionList, new PriorityQueue<AIAction>(new AIActionPriorityComparer()));

            PriorityQueue<AIAction> actions = gameObject.FireEvent(getActionEventBuilder).GetValue<PriorityQueue<AIAction>>(EventParameter.AIActionList);
            if (actions.Count > 0)
                desiredDirection = actions[0].ActionToTake();

            getActionEventBuilder.Release();

            if (desiredDirection == MoveDirection.None)
                gameObject.FireEvent(GameEventPool.Get(GameEventId.SkipTurn)).Release();
            else
                gameObject.FireEvent(GameEventPool.Get(GameEventId.MoveKeyPressed)
                    .With(EventParameter.InputDirection, desiredDirection)).Release();

            //if (desiredDirection == MoveDirection.None)
            //    FireEvent(Self, GameEventPool.Get(GameEventId.SkipTurn));

            GameEvent checkForEnergy = GameEventPool.Get(GameEventId.HasEnoughEnergyToTakeATurn)
                    .With(EventParameter.TakeTurn, false);
            gameObject.FireEvent(checkForEnergy);
            gameEvent.Paramters[EventParameter.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameter.TakeTurn];
            checkForEnergy.Release();
        }
    }
}