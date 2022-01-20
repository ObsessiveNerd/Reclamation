using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelection : GameService
{
    public void EndTileSelection(Point currentTilePos)
    {
        if(!m_Tiles.ContainsKey(currentTilePos)) return;

            m_Tiles[currentTilePos].RemoveComponent(typeof(SelectedTile));
            m_Tiles[currentTilePos].CleanupComponents();
    }

    public Point SelectTileInNewDirection(Point currentTilePos, MoveDirection moveDirection)
    {
        if (m_Tiles.ContainsKey(currentTilePos))
        {
            m_Tiles[currentTilePos].RemoveComponent(typeof(SelectedTile));
            m_Tiles[currentTilePos].CleanupComponents();
        }

        Point newPoint = GetTilePointInDirection(currentTilePos, moveDirection);
        if (m_Tiles.ContainsKey(newPoint))
        {
            m_Tiles[newPoint].AddComponent(new SelectedTile());
            m_Tiles[newPoint].CleanupComponents();
        }

        return newPoint;
    }

    private void SelectTile(Point p)
    {
        //IEntity entity = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
        //IEntity target = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameters.Target));

        //Point p = m_EntityToPointMap[target == null ? entity : target];

        m_Tiles[p].AddComponent(new SelectedTile());
        m_Tiles[p].CleanupComponents();

        //gameEvent.Paramters[EventParameters.TilePosition] = p;
    }
}
