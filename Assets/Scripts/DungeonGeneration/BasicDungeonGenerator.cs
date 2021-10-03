using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Room
{
    [SerializeField]
    private Point m_StartPoint;
    [SerializeField]
    private Point m_Size;
    private List<Point> m_Walls;
    private List<Point> m_Hallways;

    public Room(Point startPoint, Point size)
    {
        m_StartPoint = startPoint;
        m_Size = size;
        m_Hallways = new List<Point>();
        m_Walls = new List<Point>();
    }

    Point GetMiddleOfTheRoom()
    {
        int x = (m_StartPoint.x + m_Size.x - 1) - (m_Size.x / 2);
        int y = (m_StartPoint.y + m_Size.y - 1) - (m_Size.y / 2);
        return new Point(x, y);
    }

    public int SurfaceArea
    {
        get
        {
            return (m_Size.x - 2) * (m_Size.y - 2);
        }
    }

    public void CreateWalls()
    {
        List<Point> validPoints = new List<Point>();

        for (int i = m_StartPoint.x; i < m_StartPoint.x + m_Size.x; i++)
        {
            for (int j = m_StartPoint.y; j < m_StartPoint.y + m_Size.y; j++)
            {
                if (i == m_StartPoint.x || i == m_StartPoint.x + m_Size.x - 1 || j == m_StartPoint.y || j == m_StartPoint.y + m_Size.y - 1)
                {
                    Spawner.Spawn(EntityFactory.CreateEntity("Wall"), new Point(i, j));
                    m_Walls.Add(new Point(i, j));
                }
                else
                    validPoints.Add(new Point(i, j));
            }
        }

        EventBuilder sendValidPoints = new EventBuilder(GameEventId.AddValidPoints)
                                        .With(EventParameters.Value, validPoints);

        World.Instance.Self.FireEvent(sendValidPoints.CreateEvent());
    }

    public void CreateDoors()
    {
        foreach (var wall in m_Walls)
        {
            Point up = new Point(wall.x, wall.y + 1);
            Point down = new Point(wall.x, wall.y - 1);

            Point left = new Point(wall.x - 1, wall.y);
            Point right = new Point(wall.x + 1, wall.y);

            if ((WorldUtility.GetEntityAtPosition(up, false) != null && WorldUtility.GetEntityAtPosition(down, false) != null) ||
                (WorldUtility.GetEntityAtPosition(right, false) != null && WorldUtility.GetEntityAtPosition(left, false) != null))
            {

                EventBuilder getEntity = new EventBuilder(GameEventId.GetEntityOnTile)
                                            .With(EventParameters.TilePosition, wall)
                                            .With(EventParameters.Entity, null)
                                            .With(EventParameters.IncludeSelf, false);

                string entityId = World.Instance.Self.FireEvent(getEntity.CreateEvent()).GetValue<string>(EventParameters.Entity);
                if (string.IsNullOrEmpty(entityId) && RecRandom.Instance.GetRandomValue(0, 100) > 50)
                    Spawner.Spawn(EntityFactory.CreateEntity("Door"), wall);
            }
        }
    }

    public void CreateHallwayToRoom(Room otherRoom)
    {
        int hallwayDirection = RecRandom.Instance.GetRandomValue(0, 2);
        if (hallwayDirection == 1)
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
        int x = -1;
        int y = -1;
        //while(true)
        //{
            x = RecRandom.Instance.GetRandomValue(m_StartPoint.x + 2, (m_StartPoint.x + m_Size.x) - 2);
            y = RecRandom.Instance.GetRandomValue(m_StartPoint.y + 2, (m_StartPoint.y + m_Size.y) - 2);

            var entity = WorldUtility.GetEntityAtPosition(new Point(x, y), false);
            EventBuilder getPathfindingData = new EventBuilder(GameEventId.PathfindingData)
                                                .With(EventParameters.Weight, 0)
                                                .With(EventParameters.BlocksMovement, false);

        //    bool blocksMovement = World.Instance.Self.FireEvent(getPathfindingData.CreateEvent()).GetValue<bool>(EventParameters.BlocksMovement);
        //    if (!blocksMovement)
        //        break;
        //}
        return new Point(x, y);
    }

    public void ClearRoom()
    {
        for (int x = m_StartPoint.x + 1; x < (m_StartPoint.x + m_Size.x) - 1; x++)
        {
            for (int y = m_StartPoint.y + 1; y < (m_StartPoint.y + m_Size.y) - 1; y++)
            {
                EventBuilder builder = new EventBuilder(GameEventId.DestroyObject)
                                     .With(EventParameters.Point, new Point(x, y));

                World.Instance.Self.FireEvent(World.Instance.Self, builder.CreateEvent());
            }
        }
    }

    public void ClearHallways()
    {
        foreach (Point p in m_Hallways)
        {
            EventBuilder builder = new EventBuilder(GameEventId.DestroyObject)
                                      .With(EventParameters.Point, p);
            World.Instance.Self.FireEvent(World.Instance.Self, builder.CreateEvent());
        }

        EventBuilder sendValidPoints = new EventBuilder(GameEventId.AddValidPoints)
                                        .With(EventParameters.Value, m_Hallways);

        World.Instance.Self.FireEvent(sendValidPoints.CreateEvent());

    }

    void SurroundPointWithWalls(Point p)
    {
        for (int x = p.x - 1; x <= p.x + 1; x++)
        {
            for (int y = p.y - 1; y <= p.y + 1; y++)
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
    public List<Room> Rooms { get; internal set; }

    private List<DungeonPartition> m_LeafNodes = new List<DungeonPartition>();
    private DungeonPartition m_Root;
    private DungeonMetaData m_DMD;
    private int m_MinRoomSize = 6;
    private DungeonGenerationResult m_Result;
    Dictionary<ItemRarity, List<string>> m_ItemRarityToBPName = new Dictionary<ItemRarity, List<string>>();

    public BasicDungeonGenerator(int rows, int columns)
    {
        Rooms = new List<Room>();
        m_Root = new DungeonPartition(new Point(0, 0), new Point(columns, rows));

        foreach(var bp in EntityFactory.InventoryEntities)
        {
            IEntity e = EntityFactory.CreateEntity(bp);
            ItemRarity rarity = e.FireEvent(new GameEvent(GameEventId.GetRarity, new KeyValuePair<string, object>(EventParameters.Rarity, null))).GetValue<ItemRarity>(EventParameters.Rarity);
            if (!m_ItemRarityToBPName.ContainsKey(rarity))
                m_ItemRarityToBPName.Add(rarity, new List<string>());
            m_ItemRarityToBPName[rarity].Add(bp);
        }
    }

    public virtual DungeonGenerationResult GenerateDungeon(DungeonMetaData metaData)
    {
        m_Result = new DungeonGenerationResult();

        m_DMD = metaData;
        SplitPartition(m_Root);

        CreateRooms();
        CreateWalls();
        CreateHallways();
        CleanupRooms();

        SpawnEnemies();
        SpawnItems();
        SpawnStairs();

        //TODO
        if (metaData.SpawnBoss)
        {
            Room randomRoom = Rooms[RecRandom.Instance.GetRandomValue(1, Rooms.Count)];
            IEntity goblin = EntityFactory.CreateEntity("RedDragon");
            Spawner.Spawn(goblin, randomRoom.GetValidPoint());
        }

        return m_Result;
    }

    void SplitPartition(DungeonPartition partition)
    {
        int minSize = Mathf.Min(partition.Size.x, partition.Size.y);
        if (minSize / 2 < m_MinRoomSize)
        {
            if (minSize >= m_MinRoomSize)
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
        {
            Room r = node.CreateRoom(m_MinRoomSize);
            Rooms.Add(r);
            m_Result.RoomData.Add(r);
        }
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

        foreach (Room room in Rooms)
            room.CreateDoors();
    }

    void CreateHallways()
    {
        for (int i = 0; i < Rooms.Count - 1; ++i)
            Rooms[i].CreateHallwayToRoom(Rooms[i + 1]);
    }

    public void Clean()
    {
        Rooms.Clear();
        m_LeafNodes.Clear();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < RecRandom.Instance.GetRandomValue(2, 6); i++)
        {
            Room randomRoom = Rooms[RecRandom.Instance.GetRandomValue(1, Rooms.Count)];
            IEntity enemy = EntityFactory.CreateEntity(EntityFactory.GetRandomMonsterBPName());
            Spawner.Spawn(enemy, randomRoom.GetValidPoint());
        }
    }

    void SpawnItems()
    {
        foreach (var room in Rooms)
        {
            bool spawnItems = RecRandom.Instance.GetRandomValue(0, 100) < 30;
            if (spawnItems)
            {
                int totalValue = RecRandom.Instance.GetRandomValue(1, 25);
                bool spawnChest = RecRandom.Instance.GetRandomValue(0, 100) < 45;
                List<string> items = GetItemsEqualToValue(totalValue);

                if (spawnChest)
                {
                    IEntity chest = EntityFactory.CreateEntity("Chest");
                    EventBuilder addItems = new EventBuilder(GameEventId.AddItems)
                                            .With(EventParameters.Items, items);
                    if (chest == null)
                        continue;

                    Debug.Log("Chest spawned");
                    chest.FireEvent(addItems.CreateEvent());
                    Spawner.Spawn(chest, room.GetValidPoint());
                }
                else
                {
                    Debug.Log("Items should be spawned");
                    foreach (var item in items)
                        Spawner.Spawn(EntityQuery.GetEntity(item), room.GetValidPoint());
                }
            }

            int environmentObjectsToSpawn = RecRandom.Instance.GetRandomValue((int)(room.SurfaceArea * .1f), (int)(room.SurfaceArea * .4f));
            for(int i = 0; i < environmentObjectsToSpawn; i++)
            {
                string bpName = EntityFactory.GetRandomEnvironmentBPName();
                var e = EntityFactory.CreateEntity(bpName);
                if(bpName == "Bookshelf")
                {
                    EventBuilder addItems = new EventBuilder(GameEventId.AddItems)
                                            .With(EventParameters.Items, new List<string>(){
                                                "Spellbook"
                                            });
                    e.FireEvent(addItems.CreateEvent());
                }

                Spawner.Spawn(e, room.GetValidPoint());
            }
        }
    }

    void SpawnStairs()
    {
        if (m_DMD.StairsUp)
            Spawner.Spawn(EntityFactory.CreateEntity("StairsUp"), Rooms[0].GetValidPoint());

        if (m_DMD.StairsDown)
        {
            int stairsDownRoomIndex = RecRandom.Instance.GetRandomValue(1, Rooms.Count);
            Spawner.Spawn(EntityFactory.CreateEntity("StairsDown"), Rooms[stairsDownRoomIndex].GetValidPoint());
            m_Result.StairsDownRoomIndex = stairsDownRoomIndex;
        }
    }

    List<string> GetItemsEqualToValue(int value)
    {
        List<string> returnedEntities = new List<string>();
        int total = 0;

        while (CanGetItemOfRarity(ItemRarity.Mythic, value, total))
            returnedEntities.Add(GetItemOfSpecificRarity(ItemRarity.Mythic, ref total));

        while (CanGetItemOfRarity(ItemRarity.Epic, value, total))
            returnedEntities.Add(GetItemOfSpecificRarity(ItemRarity.Epic, ref total));

        while (CanGetItemOfRarity(ItemRarity.Rare, value, total))
            returnedEntities.Add(GetItemOfSpecificRarity(ItemRarity.Rare, ref total));

        while (CanGetItemOfRarity(ItemRarity.Uncommon, value, total))
            returnedEntities.Add(GetItemOfSpecificRarity(ItemRarity.Uncommon, ref total));

        while (CanGetItemOfRarity(ItemRarity.Common, value, total))
            returnedEntities.Add(GetItemOfSpecificRarity(ItemRarity.Common, ref total));

        return returnedEntities;
    }

    string GetItemOfSpecificRarity(ItemRarity rarity, ref int total)
    {
        total += (int)rarity;
        int listCount = m_ItemRarityToBPName[rarity].Count;
        string bpName = m_ItemRarityToBPName[rarity][RecRandom.Instance.GetRandomValue(0, listCount)];
        return EntityFactory.CreateEntity(bpName).ID;
    }

    bool CanGetItemOfRarity(ItemRarity rarity, int desiredTotal, int currentTotalValue)
    {
        return desiredTotal - currentTotalValue >= (int)rarity && m_ItemRarityToBPName.ContainsKey(rarity);
    }
}
