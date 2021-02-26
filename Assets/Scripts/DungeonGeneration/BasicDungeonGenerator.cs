using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private Point m_StartPoint;
    private Point m_Size;
    private List<Point> m_Hallways;

    public Room(Point startPoint, Point size)
    {
        m_StartPoint = startPoint;
        m_Size = size;
        m_Hallways = new List<Point>();
    }

    Point GetMiddleOfTheRoom()
    {
        int x = (m_StartPoint.x + m_Size.x - 1) - (m_Size.x / 2);
        int y = (m_StartPoint.y + m_Size.y - 1) - (m_Size.y / 2);
        return new Point(x, y);
    }

    public void CreateWalls()
    {
        for (int i = m_StartPoint.x; i < m_StartPoint.x + m_Size.x; i++)
        {
            for (int j = m_StartPoint.y; j < m_StartPoint.y + m_Size.y; j++)
            {
                if (i == m_StartPoint.x || i == m_StartPoint.x + m_Size.x - 1 || j == m_StartPoint.y || j == m_StartPoint.y + m_Size.y - 1)
                    Spawner.Spawn(EntityFactory.CreateEntity("Wall"), new Point(i, j));
            }
        }
    }

    public void CreateHallwayToRoom(Room otherRoom)
    {
        int hallwayDirection = RecRandom.Instance.GetRandomValue(0, 2);
        if(hallwayDirection == 1)
        {
            Point midPoint = GetMiddleOfTheRoom();
            Point otherMidPoint = otherRoom.GetMiddleOfTheRoom();

            int xDirection = (int)Mathf.Sign(otherMidPoint.x - midPoint.x);
            for (int x = midPoint.x; x != otherMidPoint.x; x += xDirection)
            {
                Point hallPoint = new Point(x, midPoint.y);
                EventBuilder builder = new EventBuilder(GameEventId.DestroyObject)
                                      .With(EventParameters.Point, hallPoint);
                m_Hallways.Add(hallPoint);

                Point above = new Point(x, hallPoint.y + 1);
                Point below = new Point(x, hallPoint.y - 1);

                Spawner.Spawn(EntityFactory.CreateEntity("Wall"), above);
                Spawner.Spawn(EntityFactory.CreateEntity("Wall"), below);

                World.Instance.Self.FireEvent(World.Instance.Self, builder.CreateEvent());
            }

            SurroundPointWithWalls(new Point(otherMidPoint.x, midPoint.y));

            int yDirection = (int)Mathf.Sign(otherMidPoint.y - midPoint.y);
            for (int y = midPoint.y; y != otherMidPoint.y; y += yDirection)
            {
                Point hallPoint = new Point(otherMidPoint.x, y);
                EventBuilder builder = new EventBuilder(GameEventId.DestroyObject)
                                         .With(EventParameters.Point, hallPoint);
                m_Hallways.Add(hallPoint);

                Point left = new Point(hallPoint.x + 1, y);
                Point right = new Point(hallPoint.x - 1, y);

                Spawner.Spawn(EntityFactory.CreateEntity("Wall"), left);
                Spawner.Spawn(EntityFactory.CreateEntity("Wall"), right);

                World.Instance.Self.FireEvent(World.Instance.Self, builder.CreateEvent());
            }
        }
        else
        {
            Point midPoint = GetMiddleOfTheRoom();
            Point otherMidPoint = otherRoom.GetMiddleOfTheRoom();
            int yDirection = (int)Mathf.Sign(otherMidPoint.y - midPoint.y);
            for (int y = midPoint.y; y != otherMidPoint.y; y += yDirection)
            {
                Point hallPoint = new Point(midPoint.x, y);
                EventBuilder builder = new EventBuilder(GameEventId.DestroyObject)
                                         .With(EventParameters.Point, hallPoint);
                m_Hallways.Add(hallPoint);

                Point left = new Point(hallPoint.x + 1, y);
                Point right = new Point(hallPoint.x - 1, y);

                Spawner.Spawn(EntityFactory.CreateEntity("Wall"), left);
                Spawner.Spawn(EntityFactory.CreateEntity("Wall"), right);

                World.Instance.Self.FireEvent(World.Instance.Self, builder.CreateEvent());
            }

            SurroundPointWithWalls(new Point(midPoint.x, otherMidPoint.y));

            int xDirection = (int)Mathf.Sign(otherMidPoint.x - midPoint.x);
            for (int x = midPoint.x; x != otherMidPoint.x; x += xDirection)
            {
                Point hallPoint = new Point(x, otherMidPoint.y);
                EventBuilder builder = new EventBuilder(GameEventId.DestroyObject)
                                      .With(EventParameters.Point, hallPoint);
                m_Hallways.Add(hallPoint);

                Point above = new Point(x, hallPoint.y + 1);
                Point below = new Point(x, hallPoint.y - 1);

                Spawner.Spawn(EntityFactory.CreateEntity("Wall"), above);
                Spawner.Spawn(EntityFactory.CreateEntity("Wall"), below);

                World.Instance.Self.FireEvent(World.Instance.Self, builder.CreateEvent());
            }
        }
    }

    public Point GetValidPoint()
    {
        int x = RecRandom.Instance.GetRandomValue(m_StartPoint.x + 1, (m_StartPoint.x + m_Size.x) - 1);
        int y = RecRandom.Instance.GetRandomValue(m_StartPoint.y + 1, (m_StartPoint.y + m_Size.y) - 1);

        return new Point(x, y);
    }

    public void ClearRoom()
    {
        for(int x = m_StartPoint.x + 1; x < (m_StartPoint.x + m_Size.x) - 1; x++)
        {
            for(int y = m_StartPoint.y + 1; y < (m_StartPoint.y + m_Size.y) - 1; y++)
            {
                EventBuilder builder = new EventBuilder(GameEventId.DestroyObject)
                                     .With(EventParameters.Point, new Point(x, y));

                World.Instance.Self.FireEvent(World.Instance.Self, builder.CreateEvent());
            }
        }
    }

    public void ClearHallways()
    {
        foreach(Point p in m_Hallways)
        {
            EventBuilder builder = new EventBuilder(GameEventId.DestroyObject)
                                      .With(EventParameters.Point, p);
            World.Instance.Self.FireEvent(World.Instance.Self, builder.CreateEvent());
        }
    }

    void SurroundPointWithWalls(Point p)
    {
        for(int x = p.x - 1; x <= p.x + 1; x++)
        {
            for(int y = p.y - 1; y <= p.y + 1; y++)
            {
                Spawner.Spawn(EntityFactory.CreateEntity("Wall"), new Point(x, y));
            }
        }
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

    public Room CreateRoom(int minRoomSize)
    {
        int roomWidth = RecRandom.Instance.GetRandomValue(minRoomSize, Size.x - 1);
        int roomHeight = RecRandom.Instance.GetRandomValue(minRoomSize, Size.y - 1);

        int roomStartX = RecRandom.Instance.GetRandomValue(StartPoint.x, (StartPoint.x + Size.x) - roomWidth);
        int roomStartY = RecRandom.Instance.GetRandomValue(StartPoint.y, (StartPoint.y + Size.y) - roomHeight);

        return new Room(new Point(roomStartX, roomStartY), new Point(roomWidth, roomHeight));
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

    List<DungeonPartition> m_LeafNodes = new List<DungeonPartition>();

    public List<Room> Rooms { get; internal set; }

    public BasicDungeonGenerator(GameObject tilePrefab)
    {
        m_TilePrefab = tilePrefab;
        m_Vertical = (int)Camera.main.orthographicSize;
        m_Horizontal = (int)(m_Vertical * Camera.main.aspect);
        m_Columns = m_Horizontal * 2;
        m_Rows = m_Vertical * 2;
        Rooms = new List<Room>();
    }

    public virtual void GenerateDungeon(Dictionary<Point, Actor> pointToTileMap)
    {
        CreateTiles(pointToTileMap);
        DungeonPartition root = new DungeonPartition(new Point(0, 0), new Point(m_Columns, m_Rows));
        SplitPartition(root);

        CreateRooms();
        CreateWalls();
        CreateHallways();
        CleanupRooms();
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

    void CreateRooms()
    {
        foreach (DungeonPartition node in m_LeafNodes)
            Rooms.Add(node.CreateRoom(m_MinRoomSize));
    }

    void CreateWalls()
    {
        foreach (Room room in Rooms)
            room.CreateWalls();
    }

    void CleanupRooms()
    {
        foreach (Room room in Rooms)
            room.ClearRoom();

        foreach (Room room in Rooms)
            room.ClearHallways();
    }

    void CreateHallways()
    {
        for(int i = 0; i < Rooms.Count - 1; ++i)
            Rooms[i].CreateHallwayToRoom(Rooms[i + 1]);
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
