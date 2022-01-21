using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelection : GameService
{
    public void EndTileSelection(Point currentTilePos)
    {
        if(!m_Tiles.ContainsKey(currentTilePos)) return;

            m_TileEntity[currentTilePos].RemoveComponent(typeof(SelectedTile));
            m_TileEntity[currentTilePos].CleanupComponents();
    }

    public Point SelectTileInNewDirection(Point currentTilePos, MoveDirection moveDirection)
    {
        if (m_Tiles.ContainsKey(currentTilePos))
        {
            m_TileEntity[currentTilePos].RemoveComponent(typeof(SelectedTile));
            m_TileEntity[currentTilePos].CleanupComponents();
        }

        Point newPoint = GetTilePointInDirection(currentTilePos, moveDirection);
        if (m_Tiles.ContainsKey(newPoint))
        {
            m_TileEntity[newPoint].AddComponent(new SelectedTile());
            m_TileEntity[newPoint].CleanupComponents();
        }

        return newPoint;
    }

    public void SelectTile(Point p)
    {
        //IEntity entity = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
        //IEntity target = Services.EntityMapService.GetEntity(gameEvent.GetValue<string>(EventParameters.Target));

        //Point p = m_EntityToPointMap[target == null ? entity : target];

        m_TileEntity[p].AddComponent(new SelectedTile());
        m_TileEntity[p].CleanupComponents();

        //gameEvent.Paramters[EventParameters.TilePosition] = p;
    }
}
