﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackController : InputControllerBase
{
    Point m_TileSelection;

    //Todo: I could probably initialize this with a specifc attack bow/spell/thrown object/whatever and get a lot of reuse out of this
    public RangedAttackController(IEntity self, Point startTileSelection)
    {
        Init(self);
        m_TileSelection = startTileSelection;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.UpdateEntity)
        {
            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                GameEvent moveSelection = new GameEvent(GameEventId.SelectNewTileInDirection, new KeyValuePair<string, object>(EventParameters.InputDirection, desiredDirection),
                                                                                                    new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection));
                FireEvent(World.Instance.Self, moveSelection);
                m_TileSelection = (Point)moveSelection.Paramters[EventParameters.TilePosition];
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInputController(Self));
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.EndSelection, new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection)));
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
                gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }

            //Todo: implement actual call for attack and then use the energy like normal in the PlayerInput
            gameEvent.Paramters[EventParameters.TakeTurn] = false;
        }
    }
}
