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
    public bool SpawnEnemies;

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
                        case LevelMetaData.MonsterTypes:
                            if (keyValue[1].Contains("None"))
                                SpawnEnemies = false;
                            else
                                SpawnEnemies = true;
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
        RegisteredEvents.Add(GameEventId.GetCurrentLevel);

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
            Factions.Initialize();

            m_DungeonGenerator = new BasicDungeonGenerator(World.Instance.MapRows, World.Instance.MapColumns);

            LoadOrCreateDungeon();
            SpawnPlayers();

            FireEvent(Self, new GameEvent(GameEventId.SaveLevel));
        }
        else if(gameEvent.ID == GameEventId.GetCurrentLevel)
        {
            gameEvent.Paramters[EventParameters.Level] = m_CurrentLevel;
        }
        else if (gameEvent.ID == GameEventId.MoveUp)
        {
            m_TimeProgression.SetPostFrameCallback(() =>
            {
                SaveCurrentLevel();
                CleanTiles();
                m_CurrentLevel--;
                LoadOrCreateDungeon();
                MovePlayersToCurrentFloor(false);

            });
        }
        else if (gameEvent.ID == GameEventId.MoveDown)
        {
            m_TimeProgression.SetPostFrameCallback(() =>
            {
                SaveCurrentLevel();
                CleanTiles();
                m_CurrentLevel++;
                LoadOrCreateDungeon();
                MovePlayersToCurrentFloor(true);

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
            SaveCurrentLevel();
        }

        if (gameEvent.ID == GameEventId.LoadLevel)
        {
            m_CurrentLevel = gameEvent.GetValue<int>(EventParameters.Level);
            //LoadOrCreateDungeon();
            //TODO
        }
    }

    void SaveCurrentLevel()
    {
        DungeonGenerationResult level = m_DungeonLevelMap[m_CurrentLevel];
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

    //void SetupNewLevel()
    //{
    //    Factions.Initialize();

    //    if (m_DungeonLevelMap.ContainsKey(m_CurrentLevel))
    //    {
    //        DungeonGenerationResult dungeonLevel = m_DungeonLevelMap[m_CurrentLevel];
    //        foreach (var room in dungeonLevel.RoomData)
    //            m_DungeonGenerator.Rooms.Add(room);

    //        foreach (string entityData in dungeonLevel.Entities)
    //        {
    //            IEntity entity = EntityFactory.ParseEntityData(entityData);
    //            if (entity != null)
    //            {
    //                EventBuilder getPoint = new EventBuilder(GameEventId.GetPoint)
    //                                        .With(EventParameters.Value, Point.InvalidPoint);
    //                Point p = FireEvent(entity, getPoint.CreateEvent()).GetValue<Point>(EventParameters.Value);
    //                if (p != Point.InvalidPoint)
    //                    Spawner.Spawn(entity, p);
    //            }
    //        }

    //        foreach (var player in m_Players)
    //            FireEvent(player, new GameEvent(GameEventId.InitFOV));
    //    }
    //    else
    //    {
    //        DungeonMetaData dmd = new DungeonMetaData($"{LevelMetaData.MetadataPath}/{m_CurrentLevel}.lvl");
    //        m_DungeonGenerator.GenerateDungeon(dmd);

    //        SpawnPlayers();
    //    }

    //    FireEvent(Self, new GameEvent(GameEventId.UpdateWorldView));
    //}

    void LoadOrCreateDungeon()
    {
        if (m_DungeonLevelMap.ContainsKey(m_CurrentLevel))
        {
            DungeonGenerationResult dungeonLevel = m_DungeonLevelMap[m_CurrentLevel];
            foreach (var room in dungeonLevel.RoomData)
                m_DungeonGenerator.Rooms.Add(room);

            foreach (string entityData in dungeonLevel.Entities)
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
        }
        else
        {
            DungeonMetaData dmd = new DungeonMetaData($"{LevelMetaData.MetadataPath}/{m_CurrentLevel}.lvl");
            DungeonGenerationResult dr = m_DungeonGenerator.GenerateDungeon(dmd);
            m_DungeonLevelMap.Add(m_CurrentLevel, dr);
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

    void MovePlayersToCurrentFloor(bool movingDown)
    {
        var playerIds = new List<string>();
        playerIds.AddRange(m_Players.Select(p => p.ID));

        foreach (var player in playerIds)
        {
            var entity = EntityQuery.GetEntity(player);
            if(movingDown)
                Spawner.Move(entity, m_DungeonGenerator.Rooms[0].GetValidPoint());
            else
                Spawner.Move(entity, m_DungeonGenerator.Rooms[m_DungeonLevelMap[m_CurrentLevel].StairsDownRoomIndex].GetValidPoint());
            //Spawner.Despawn(entity);
            //Spawner.Spawn(entity, m_DungeonGenerator.Rooms[0].GetValidPoint());
            FireEvent(entity, new GameEvent(GameEventId.InitFOV));
        }

        //FireEvent(Self, new GameEvent(GameEventId.ProgressTime));
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
