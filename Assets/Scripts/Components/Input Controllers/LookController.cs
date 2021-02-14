using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookController : InputControllerBase
{
    Point m_TileSelection;

    public override void Init(IEntity self)
    {
        base.Init(self);
        GameEvent selectTile = new GameEvent(GameEventId.SelectTile, new KeyValuePair<string, object>(EventParameters.Entity, Self.ID),
                                                                               new KeyValuePair<string, object>(EventParameters.Target, null),
                                                                               new KeyValuePair<string, object>(EventParameters.TilePosition, null));
        FireEvent(World.Instance.Self, selectTile);

        m_TileSelection = (Point)selectTile.Paramters[EventParameters.TilePosition];

        GameEvent showTileInfo = new GameEvent(GameEventId.ShowTileInfo, new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection));
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
                Self.AddComponent(new PlayerInputController());
                FireEvent(World.Instance.Self, new GameEvent(GameEventId.EndSelection, new KeyValuePair<string, object>(EventParameters.TilePosition, m_TileSelection)));
                gameEvent.Paramters[EventParameters.UpdateWorldView] = true;
                //gameEvent.Paramters[EventParameters.CleanupComponents] = true;
            }
        }
    }
}

public class DTO_LookController : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new LookController();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(PlayerInputController);
    }
}