using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDungeonGenerator : IDungeonGenerator
{
    GameObject m_TilePrefab;
    int m_Vertical, m_Horizontal, m_Columns, m_Rows;

    public BasicDungeonGenerator(int seed, GameObject tilePrefab)
    {
        m_TilePrefab = tilePrefab;
        m_Vertical = (int)Camera.main.orthographicSize;
        m_Horizontal = (int)(m_Vertical * Camera.main.aspect);
        m_Columns = m_Horizontal * 2;
        m_Rows = m_Vertical * 2;
    }

    public virtual void GenerateDungeon(Dictionary<Point, Actor> pointToTileMap, Dictionary<IEntity, GameObject> tileToGameObjectMap)
    {
        for (int i = 0; i < m_Columns; i++)
        {
            for (int j = 0; j < m_Rows; j++)
            {
                CreateTile(i, j, m_Horizontal, m_Vertical, pointToTileMap, tileToGameObjectMap);
            }
        }
    }

    protected void CreateTile(int x, int y, float screenHorizontal, float screenVertical, Dictionary<Point, Actor> pointToTileMap, Dictionary<IEntity, GameObject> tileToGameObjectMap)
    {
        GameObject tile = UnityEngine.GameObject.Instantiate(m_TilePrefab);
        tile.transform.position = new Vector2(x - (screenHorizontal - 0.5f), y - (screenVertical - 0.5f));
        tile.transform.parent = UnityEngine.GameObject.Find("World").transform;

        Actor actor = new Actor("Tile");
        actor.AddComponent(new Tile(actor, new Point(x, y)));
        actor.AddComponent(new Renderer(tile.GetComponent<SpriteRenderer>()));
        actor.CleanupComponents();

        tileToGameObjectMap.Add(actor, tile);
        pointToTileMap.Add(new Point(x, y), actor);
    }
}
