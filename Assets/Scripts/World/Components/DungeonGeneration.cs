using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DungeonMetaData
{
    public bool StairsUp;
    public bool StairsDown;

    public DungeonMetaData(string dataPath)
    {
        if (File.Exists(dataPath))
        {
            using (StreamReader reader = new StreamReader(dataPath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] keyValue = line.Split(':');
                    switch (keyValue[0])
                    {
                        case LevelMetaData.StairsUp:
                            StairsUp = bool.Parse(keyValue[1]);
                            break;
                        case LevelMetaData.StairsDown:
                            StairsDown = bool.Parse(keyValue[1]);
                            break;
                    }
                }
            }
        }
        else
        {
            StairsUp = true;
            StairsDown = true;
        }
    }
}

public class DungeonGeneration : WorldComponent
{
    GameObject m_TilePrefab;
    IDungeonGenerator m_DungeonGenerator;
    public string Seed;
    int m_Vertical, m_Horizontal;

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.StartWorld);
        RegisteredEvents.Add(GameEventId.GetRandomValidPoint);
        RegisteredEvents.Add(GameEventId.AddValidPoints);
        RegisteredEvents.Add(GameEventId.MoveDown);
        RegisteredEvents.Add(GameEventId.MoveUp);
        RegisteredEvents.Add(GameEventId.SaveLevel);
        RegisteredEvents.Add(GameEventId.LoadLevel);

        m_Vertical = (int)Camera.main.orthographicSize;
        m_Horizontal = (int)(m_Vertical * Camera.main.aspect);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.StartWorld)
        {
            Seed = (string)gameEvent.Paramters[EventParameters.Seed];
            m_TilePrefab = (GameObject)gameEvent.Paramters[EventParameters.GameObject];
            CreateTiles(m_Tiles);

            m_DungeonGenerator = new BasicDungeonGenerator(World.Instance.MapRows, World.Instance.MapColumns);

            SetupNewLevel();
            FireEvent(Self, new GameEvent(GameEventId.SaveLevel));
        }
        else if (gameEvent.ID == GameEventId.MoveUp)
        {
            m_TimeProgression.SetPostFrameCallback(() =>
            {
                CleanTiles();
                m_CurrentLevel--;
                MoveToNewLevel();
            });
        }
        else if (gameEvent.ID == GameEventId.MoveDown)
        {
            m_TimeProgression.SetPostFrameCallback(() =>
            {
                CleanTiles();
                m_CurrentLevel++;
                MoveToNewLevel();
            });
        }
        else if (gameEvent.ID == GameEventId.GetRandomValidPoint)
        {
            int roomIndex = RecRandom.Instance.GetRandomValue(0, m_DungeonGenerator.Rooms.Count - 1);
            gameEvent.Paramters[EventParameters.Value] = m_DungeonGenerator.Rooms[roomIndex].GetValidPoint();
        }
        else if (gameEvent.ID == GameEventId.AddValidPoints)
        {
            List<Point> validPoints = gameEvent.GetValue<List<Point>>(EventParameters.Value);
            foreach (Point p in validPoints)
                m_ValidDungeonPoints.Add(p);
        }

        if (gameEvent.ID == GameEventId.SaveLevel)
        {
            SerializedLevelData level = new SerializedLevelData();
            foreach (var tile in m_Tiles.Values)
            {
                EventBuilder serializeTile = new EventBuilder(GameEventId.SerializeTile)
                                             .With(EventParameters.Value, level);
                FireEvent(tile, serializeTile.CreateEvent());
            }

            foreach (Room room in m_DungeonGenerator.Rooms)
                level.RoomData.Add(room);

            SaveSystem.Instance.SaveLevel(level, m_CurrentLevel);
        }

        if (gameEvent.ID == GameEventId.LoadLevel)
        {
            int levelToLoad = gameEvent.GetValue<int>(EventParameters.Level);

            //TODO
        }
    }

    void SetupNewLevel()
    {
        SerializedLevelData data = SaveSystem.Instance.LoadLevel(m_CurrentLevel);
        Factions.Initialize();

        if (data != null)
        {
            foreach (var room in data.RoomData)
                m_DungeonGenerator.Rooms.Add(room);

            foreach (string entityData in data.Entities)
            {
                IEntity entity = EntityFactory.ParseEntityData(entityData);
                if (entity != null)
                {
                    EventBuilder getPoint = new EventBuilder(GameEventId.GetPoint)
                                            .With(EventParameters.Value, Point.InvalidPoint);
                    Point p = FireEvent(entity, getPoint.CreateEvent()).GetValue<Point>(EventParameters.Value);
                    if (p != Point.InvalidPoint)
                        Spawner.Spawn(entity, p);
                }
            }

            foreach (var player in m_Players)
                FireEvent(player, new GameEvent(GameEventId.InitFOV));
        }
        else
        {
            m_CurrentLevel = 1;
            DungeonMetaData dmd = new DungeonMetaData($"{LevelMetaData.MetadataPath}/{m_CurrentLevel}.lvl");
            m_DungeonGenerator.GenerateDungeon(dmd);

            SpawnPlayers();
            //SpawnEnemies();
            SpawnItems(dmd);
        }

        //MovePlayersToCurrentFloor();
        FireEvent(Self, new GameEvent(GameEventId.UpdateWorldView));
    }

    void MoveToNewLevel()
    {
        SerializedLevelData data = SaveSystem.Instance.LoadLevel(m_CurrentLevel);
        if (data != null)
        {
            foreach (var room in data.RoomData)
                m_DungeonGenerator.Rooms.Add(room);

            foreach (string entityData in data.Entities)
            {
                IEntity entity = EntityFactory.ParseEntityData(entityData);
                if (entity != null)
                {
                    EventBuilder getPoint = new EventBuilder(GameEventId.GetPoint)
                                            .With(EventParameters.Value, Point.InvalidPoint);
                    Point p = FireEvent(entity, getPoint.CreateEvent()).GetValue<Point>(EventParameters.Value);
                    if (p != Point.InvalidPoint)
                        Spawner.Spawn(entity, p);
                }
            }

            MovePlayersToCurrentFloor();

            foreach (var player in m_Players)
                FireEvent(player, new GameEvent(GameEventId.InitFOV));
        }
        else
        {
            DungeonMetaData dmd = new DungeonMetaData($"{LevelMetaData.MetadataPath}/{m_CurrentLevel}.lvl");
            m_DungeonGenerator.GenerateDungeon(dmd);
            SpawnEnemies();
            SpawnItems(dmd);
            MovePlayersToCurrentFloor();
        }
    }


    void CleanTiles()
    {
        List<IEntity> entities = new List<IEntity>(m_EntityToPointMap.Keys);
        foreach (var entity in entities)
        {
            if (m_Players.Contains(entity))
                continue;
            if (!m_EntityToPointMap.ContainsKey(entity))
                continue;

            Point p = m_EntityToPointMap[entity];
            if (m_Tiles.ContainsKey(p))
                FireEvent(m_Tiles[p], new GameEvent(GameEventId.DestroyAll));

            m_EntityToPointMap.Remove(entity);
            m_TimeProgression.RemoveEntity(entity);
        }
        m_DungeonGenerator.Clean();
    }

    void SpawnPlayers()
    {
        for (int i = 0; i < 2; i++)
        {
            IEntity player = EntityFactory.CreateEntity("DwarfWarrior");
            FireEvent(Self, new GameEvent(GameEventId.ConvertToPlayableCharacter, new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.Entity, player.ID)));
            Spawner.Spawn(player, m_DungeonGenerator.Rooms[0].GetValidPoint());

            if (i == 0)
                player.CleanupComponents();

            FireEvent(player, new GameEvent(GameEventId.InitFOV));
        }

        FireEvent(Self, new GameEvent(GameEventId.ProgressTime));
    }

    void MovePlayersToCurrentFloor()
    {
        var playerIds = new List<string>();
        playerIds.AddRange(m_Players.Select(p => p.ID));

        foreach (var player in playerIds)
        {
            var entity = EntityQuery.GetEntity(player);
            Spawner.Move(entity, m_DungeonGenerator.Rooms[0].GetValidPoint());
            //Spawner.Despawn(entity);
            //Spawner.Spawn(entity, m_DungeonGenerator.Rooms[0].GetValidPoint());
            FireEvent(entity, new GameEvent(GameEventId.InitFOV));
        }

        //FireEvent(Self, new GameEvent(GameEventId.ProgressTime));
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < 2; i++)
        {
            Room randomRoom = m_DungeonGenerator.Rooms[RecRandom.Instance.GetRandomValue(1, m_DungeonGenerator.Rooms.Count)];
            IEntity goblin = EntityFactory.CreateEntity("GoblinWarrior");
            Spawner.Spawn(goblin, randomRoom.GetValidPoint());
        }
    }

    void SpawnItems(DungeonMetaData dmd)
    {
        Room randomRoom = m_DungeonGenerator.Rooms[RecRandom.Instance.GetRandomValue(1, m_DungeonGenerator.Rooms.Count)];
        IEntity helmet = EntityFactory.CreateEntity("BronzeHelmet");
        Spawner.Spawn(helmet, randomRoom.GetValidPoint());

        if (dmd.StairsUp)
            Spawner.Spawn(EntityFactory.CreateEntity("StairsUp"), m_DungeonGenerator.Rooms[0].GetValidPoint());
        if (dmd.StairsDown)
            Spawner.Spawn(EntityFactory.CreateEntity("StairsDown"), m_DungeonGenerator.Rooms[RecRandom.Instance.GetRandomValue(1, m_DungeonGenerator.Rooms.Count)].GetValidPoint());
    }

    void CreateTiles(Dictionary<Point, Actor> pointToTileMap)
    {
        for (int i = 0; i < World.Instance.MapColumns; i++)
        {
            for (int j = 0; j < World.Instance.MapRows; j++)
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
        m_GameObjectMap.Add(new Point(x, y), tile);

        Actor actor = new Actor("Tile");
        actor.AddComponent(new Tile(actor, new Point(x, y)));
        actor.AddComponent(new TileVisible(false));
        actor.AddComponent(new GraphicContainer("Textures/Environment/td_world_floor_cobble_b-120"));
        actor.AddComponent(new Renderer(tile.GetComponent<SpriteRenderer>()));
        actor.CleanupComponents();

        pointToTileMap.Add(new Point(x, y), actor);
    }
}
