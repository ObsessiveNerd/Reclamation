﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drunk : Component
{
    int m_DrunkRounds;
    int m_CurrentDrunkRounds;

    public Drunk(IEntity self)
    {
        Init(self);

        m_DrunkRounds = 50;

        RegisteredEvents.Add(GameEventId.BeforeMoving);
        RegisteredEvents.Add(GameEventId.EndTurn);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.BeforeMoving)
            gameEvent.Paramters[EventParameters.InputDirection] = InputUtility.GetRandomMoveDirection();

        if (gameEvent.ID == GameEventId.EndTurn)
        {
            m_CurrentDrunkRounds++;
            if(m_CurrentDrunkRounds >= m_DrunkRounds)
                Self.RemoveComponent(this);
        }
    }
}
