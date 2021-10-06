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
    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            using (new DiagnosticsTimer("AI Controller"))
            {
                MoveDirection desiredDirection = MoveDirection.None; //InputUtility.GetRandomMoveDirection(); //obviously temp

                EventBuilder getActionEventBuilder = EventBuilderPool.Get(GameEventId.GetActionToTake)
                                                        .With(EventParameters.AIActionList, new PriorityQueue<AIAction>(new AIActionPriorityComparer()));

                PriorityQueue<AIAction> actions = FireEvent(Self, getActionEventBuilder.CreateEvent()).GetValue<PriorityQueue<AIAction>>(EventParameters.AIActionList);
                if (actions.Count > 0)
                    desiredDirection = actions[0].ActionToTake();

                if (desiredDirection == MoveDirection.None)
                    FireEvent(Self, new GameEvent(GameEventId.SkipTurn));

                FireEvent(Self, new GameEvent(GameEventId.MoveKeyPressed, new KeyValuePair<string, object>(EventParameters.InputDirection, desiredDirection)));

                //if (desiredDirection == MoveDirection.None)
                //    FireEvent(Self, new GameEvent(GameEventId.SkipTurn));

                GameEvent checkForEnergy = new GameEvent(GameEventId.HasEnoughEnergyToTakeATurn, new KeyValuePair<string, object>(EventParameters.TakeTurn, false));
                FireEvent(Self, checkForEnergy);
                gameEvent.Paramters[EventParameters.TakeTurn] = (bool)checkForEnergy.Paramters[EventParameters.TakeTurn];
            }
        }
    }
}

public class DTO_AIController : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new AIController();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(AIController);
    }
}
