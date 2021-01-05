using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookController : InputControllerBase
{
    Point m_TileSelection;

    public LookController(IEntity self, Point startTileSelection)
    {
        Init(self);

        m_TileSelection = startTileSelection;

        GameEvent showTileInfo = new GameEvent(GameEventId.ShowInfo, new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection));
        FireEvent(World.Instance.Self, showTileInfo);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateEntity)
        {
            MoveDirection desiredDirection = InputUtility.GetMoveDirection();

            if (desiredDirection != MoveDirection.None)
            {
                GameEvent moveSelection = new GameEvent(GameEventId.SelectNewTileInDirection, new KeyValuePair<string, object>(EventParameters.InputDirection, desiredDirection),
                                                                                                    new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection));
                FireEvent(World.Instance.Self, moveSelection);
                m_TileSelection = (Point)moveSelection.Paramters[EventParameters.TilePosition];

                GameEvent showTileInfo = new GameEvent(GameEventId.ShowTileInfo, new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection));
                FireEvent(World.Instance.Self, showTileInfo);
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Self.RemoveComponent(this);
                Self.AddComponent(new PlayerInputController(Self));
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.EndSelection, new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection)));
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
                gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }
        }
    }
}
