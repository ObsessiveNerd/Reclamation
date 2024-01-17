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
    Energy m_Energy;

    public void Start()
    {
        m_Energy = GetComponent<Energy>();
    }

    void Update()
    {
        if (!IsOwner)
            return;

        MoveDirection desiredDirection = MoveDirection.None;

        GameEvent getActionEventBuilder = GameEventPool.Get(GameEventId.GetActionToTake)
                                                        .With(EventParameter.AIActionList, new PriorityQueue<AIAction>(new AIActionPriorityComparer()));

        PriorityQueue<AIAction> actions = gameObject.FireEvent(getActionEventBuilder).GetValue<PriorityQueue<AIAction>>(EventParameter.AIActionList);
        if (actions.Count > 0)
            desiredDirection = actions[0].ActionToTake();

        getActionEventBuilder.Release();

        if(desiredDirection != MoveDirection.None && m_Energy.Data.CanTakeATurn)
        {
            MoveServerRpc(desiredDirection);
            m_Energy.TakeTurn();
        }
    }
}