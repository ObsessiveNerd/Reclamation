using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelection : WorldComponent
{
    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.SelectTile);
        RegisteredEvents.Add(GameEventId.SelectNewTileInDirection);
        RegisteredEvents.Add(GameEventId.EndSelection);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.SelectTile)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            IEntity target = (IEntity)gameEvent.Paramters[EventParameters.Target];

            Point p = m_EntityToPointMap[target == null ? entity : target];

            m_Tiles[p].AddComponent(new SelectedTile());
            m_Tiles[p].CleanupComponents();

            gameEvent.Paramters[EventParameters.TilePosition] = p;
        }

        if (gameEvent.ID == GameEventId.SelectNewTileInDirection)
        {
            Point currentTilePos = (Point)gameEvent.Paramters[EventParameters.TilePosition];
            MoveDirection moveDirection = (MoveDirection)gameEvent.Paramters[EventParameters.InputDirection];

            m_Tiles[currentTilePos].RemoveComponent(typeof(SelectedTile));
            m_Tiles[currentTilePos].CleanupComponents();

            Point newPoint = GetTilePointInDirection(currentTilePos, moveDirection);

            m_Tiles[newPoint].AddComponent(new SelectedTile());
            m_Tiles[newPoint].CleanupComponents();

            gameEvent.Paramters[EventParameters.TilePosition] = newPoint;
        }

        if (gameEvent.ID == GameEventId.EndSelection)
        {
            Point currentTilePos = (Point)gameEvent.Paramters[EventParameters.TilePosition];
            m_Tiles[currentTilePos].RemoveComponent(typeof(SelectedTile));
            m_Tiles[currentTilePos].CleanupComponents();
            gameEvent.Paramters[EventParameters.TilePosition] = null;
        }
    }
}
