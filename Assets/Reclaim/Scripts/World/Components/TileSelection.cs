using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelection : GameService
{
    public void EndTileSelection(Point currentTilePos)
    {
        if (!m_Tiles.ContainsKey(currentTilePos))
            return;

        if (m_TileEntity[currentTilePos].HasComponent(typeof(SelectedTile)))
        {
            m_TileEntity[currentTilePos].RemoveComponent(typeof(SelectedTile));
            m_TileEntity[currentTilePos].CleanupComponents();
            m_ChangedTiles.Add(m_Tiles[currentTilePos]);
        }
    }

    public Point SelectTileInNewDirection(Point currentTilePos, MoveDirection moveDirection)
    {
        if (m_Tiles.ContainsKey(currentTilePos))
        {
            if (m_TileEntity[currentTilePos].HasComponent(typeof(SelectedTile)))
            {
                m_TileEntity[currentTilePos].RemoveComponent(typeof(SelectedTile));
                m_TileEntity[currentTilePos].CleanupComponents();
                m_ChangedTiles.Add(m_Tiles[currentTilePos]);
            }
        }

        Point newPoint = GetTilePointInDirection(currentTilePos, moveDirection);
        if (m_Tiles.ContainsKey(newPoint))
        {
            if (!m_TileEntity[newPoint].HasComponent(typeof(SelectedTile)))
            {
                m_TileEntity[newPoint].AddComponent(new SelectedTile());
                m_TileEntity[newPoint].CleanupComponents();
                m_ChangedTiles.Add(m_Tiles[newPoint]);
            }
        }


        return newPoint;
    }

    public void SelectTile(Point p)
    {
        if (!m_TileEntity[p].HasComponent(typeof(SelectedTile)))
        {
            m_TileEntity[p].AddComponent(new SelectedTile());
            m_TileEntity[p].CleanupComponents();

            m_ChangedTiles.Add(m_Tiles[p]);
        }
    }
}
