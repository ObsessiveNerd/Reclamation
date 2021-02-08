using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private Point m_StartPoint;
    private Point m_Size;
    private Point m_EndPoint;

    public Room()
    {

    }
}

public class DungeonPartition
{
    public Point StartPoint;
    public Point Size;
    public DungeonPartition[] Children = new DungeonPartition[2];
    public int Area { get { return Size.x * Size.y; } }

    public DungeonPartition(Point start, Point size)
    {
        StartPoint = start;
        Size = size;
    }

    public void CreateRoom()
    {
        for(int i = StartPoint.x; i < StartPoint.x + Size.x; i++)
        {
            for(int j = StartPoint.y; j < StartPoint.y + Size.y; j++)
            {
                if(i == StartPoint.x || i == StartPoint.x + Size.x - 1 || j == StartPoint.y || j == StartPoint.y + Size.y - 1)
                    Spawner.Spawn(EntityFactory.CreateEntity("Wall"), new Point(i, j));
            }
        }
    }

    public Point GetRandomPosition()
    {
        return new Point(RecRandom.Instance.GetRandomValue(StartPoint.x + 1, StartPoint.x + Size.x), RecRandom.Instance.GetRandomValue(StartPoint.y + 1, StartPoint.y + Size.y));
    }
}

public enum Direction
{
    Horizontal,
    Vertical
}

public class BasicDungeonGenerator : IDungeonGenerator
{
    GameObject m_TilePrefab;
    int m_Vertical, m_Horizontal, m_Columns, m_Rows;
    int m_MinRoomSize = 5;
    int m_MaxRoomSize = 8;

    List<DungeonPartition> m_LeafNodes = new List<DungeonPartition>();

    public BasicDungeonGenerator(int seed, GameObject tilePrefab)
    {
        m_TilePrefab = tilePrefab;
        m_Vertical = (int)Camera.main.orthographicSize;
        m_Horizontal = (int)(m_Vertical * Camera.main.aspect);
        m_Columns = m_Horizontal * 2;
        m_Rows = m_Vertical * 2;
    }

    public virtual void GenerateDungeon(Dictionary<Point, Actor> pointToTileMap, out Point spawnPoint)
    {
        CreateTiles(pointToTileMap);
        DungeonPartition root = new DungeonPartition(new Point(0, 0), new Point(m_Columns, m_Rows));
        SplitPartition(root);

        CreateWalls();

        spawnPoint = new Point(0, 0);
    }

    void SplitPartition(DungeonPartition partition)
    {
        int minSize = Mathf.Min(partition.Size.x, partition.Size.y);
        if (minSize / 2 < m_MinRoomSize)
        {
            if(minSize >= m_MinRoomSize)
                m_LeafNodes.Add(partition);
            return;
        }

        Direction randomDirection = 0;
        if (partition.Size.x / partition.Size.y >= 1.25)
            randomDirection = Direction.Vertical;
        else if (partition.Size.y / partition.Size.x >= 1.25)
            randomDirection = Direction.Horizontal;
        else
            randomDirection = (Direction)RecRandom.Instance.GetRandomValue(0, 2);

        int splitPos = 0;
        switch (randomDirection)
        {
            case Direction.Horizontal:
                splitPos = RecRandom.Instance.GetRandomValue(m_MinRoomSize, partition.Size.x - m_MinRoomSize);
                partition.Children[0] = new DungeonPartition(partition.StartPoint, new Point(partition.Size.x, splitPos));
                partition.Children[1] = new DungeonPartition(new Point(partition.StartPoint.x, partition.StartPoint.y + splitPos), new Point(partition.Size.x, partition.Size.y - splitPos));
                break;
            case Direction.Vertical:
                splitPos = RecRandom.Instance.GetRandomValue(m_MinRoomSize, partition.Size.y - m_MinRoomSize);
                partition.Children[0] = new DungeonPartition(partition.StartPoint, new Point(splitPos, partition.Size.y));
                partition.Children[1] = new DungeonPartition(new Point(partition.StartPoint.x + splitPos, partition.StartPoint.y), new Point(partition.Size.x - splitPos, partition.Size.y));
                break;
        }

        SplitPartition(partition.Children[0]);
        SplitPartition(partition.Children[1]);
    }

    void CreateWalls()
    {
        foreach (DungeonPartition node in m_LeafNodes)
            node.CreateRoom();
    }

    void CreateTiles(Dictionary<Point, Actor> pointToTileMap)
    {
        for (int i = 0; i < m_Columns; i++)
        {
            for (int j = 0; j < m_Rows; j++)
            {
                CreateTile(i, j, m_Horizontal, m_Vertical, pointToTileMap);
            }
        }
    }

    protected void CreateTile(int x, int y, float screenHorizontal, float screenVertical, Dictionary<Point, Actor> pointToTileMap)
    {
        GameObject tile = UnityEngine.GameObject.Instantiate(m_TilePrefab);
        tile.transform.position = new Vector2(x - (screenHorizontal - 0.5f), y - (screenVertical - 0.5f));
        tile.transform.parent = UnityEngine.GameObject.Find("World").transform;

        Actor actor = new Actor("Tile");
        actor.AddComponent(new Tile(actor, new Point(x, y)));
        actor.AddComponent(new TileVisible(false));
        actor.AddComponent(new GraphicContainer("Textures/td_world_floor_cobble_b-120"));
        actor.AddComponent(new Renderer(tile.GetComponent<SpriteRenderer>()));
        actor.CleanupComponents();

        pointToTileMap.Add(new Point(x, y), actor);
    }
}
