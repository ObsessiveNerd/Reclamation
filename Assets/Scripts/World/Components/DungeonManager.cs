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
    public bool SpawnBoss;
    public string TileType;

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
                        case LevelMetaData.ContainsBoss:
                            SpawnBoss = bool.Parse(keyValue[1]);
                            break;
                        case LevelMetaData.TileType:
                            TileType = keyValue[1];
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

public class DungeonManager : GameService
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
            Debug.Log("Start world called");
            Seed = (string)gameEvent.Paramters[EventParameters.Seed];
            m_TilePrefab = (GameObject)gameEvent.Paramters[EventParameters.GameObject];
            Clean();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            CreateTiles(m_Tiles);
            sw.Stop();

            m_CurrentLevel = gameEvent.GetValue<int>(EventParameters.Level);
            m_DungeonGenerator = new BasicDungeonGenerator(World.Services.MapRows, World.Services.MapColumns);

            LoadOrCreateDungeon();
            sw.Reset();
            sw.Start();
            if (gameEvent.GetValue<bool>(EventParameters.NewGame))
                SpawnPlayers();
            else
            {
                foreach (var player in m_Players)
                    FireEvent(player, GameEventPool.Get(GameEventId.InitFOV)).Release();
                FireEvent(Self, GameEventPool.Get(GameEventId.UpdateCamera)).Release();
            }

            FireEvent(Self, GameEventPool.Get(GameEventId.SaveLevel)).Release();
            m_TimeProgression.Resume();
            sw.Stop();

        }
        else if (gameEvent.ID == GameEventId.GetCurrentLevel)
        {
            gameEvent.Paramters[EventParameters.Level] = m_CurrentLevel;
        }
        else if (gameEvent.ID == GameEventId.MoveUp)
        {
            m_TimeProgression.SetPostFrameCallback(() =>
            {
                CachePlayers();
                SaveCurrentLevel();
                CleanTiles();
                m_CurrentLevel--;
                LoadOrCreateDungeon();
                MovePlayersToCurrentFloor(false);
                FireEvent(Self, GameEventPool.Get(GameEventId.UpdateCamera));

            });
        }
        else if (gameEvent.ID == GameEventId.MoveDown)
        {
            m_TimeProgression.SetPostFrameCallback(() =>
            {
                CachePlayers();
                SaveCurrentLevel();
                CleanTiles();
                m_CurrentLevel++;
                LoadOrCreateDungeon();
                MovePlayersToCurrentFloor(true);
                FireEvent(Self, GameEventPool.Get(GameEventId.UpdateCamera));
            });
        }
        else if (gameEvent.ID == GameEventId.GetRandomValidPoint)
        {
            if (m_DungeonGenerator == null)
                gameEvent.Paramters[EventParameters.Value] = new Point(1, 1);
            else
            {
                int roomIndex = RecRandom.Instance.GetRandomValue(0, m_DungeonGenerator.Rooms.Count - 1);
                gameEvent.Paramters[EventParameters.Value] = m_DungeonGenerator.Rooms[roomIndex].GetValidPoint(null);
            }
        }
        else if (gameEvent.ID == GameEventId.AddValidPoints)
        {
            HashSet<Point> validPoints = gameEvent.GetValue<HashSet<Point>>(EventParameters.Value);
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
        }
    }

    List<string> m_PlayerBlueprintCache = new List<string>();
    void CachePlayers()
    {
        m_PlayerBlueprintCache.Clear();
        foreach (var player in m_Players)
        {
            m_PlayerBlueprintCache.Add(player.ID);
            Spawner.Despawn(player);
        }
    }

    void SaveCurrentLevel()
    {
        DungeonGenerationResult level = null;
        if (m_DungeonLevelMap.ContainsKey(m_CurrentLevel))
        {
            level = m_DungeonLevelMap[m_CurrentLevel];
            level.ClearData();
        }
        else if (SaveSystem.Instance.LoadLevel(m_CurrentLevel) != null)
        {
            level = SaveSystem.Instance.LoadLevel(m_CurrentLevel);
            level.ClearData();
            m_DungeonLevelMap.Add(m_CurrentLevel, level);
        }
        else
            return;

        foreach (var tile in m_Tiles.Values)
        {
            GameEvent serializeTile = GameEventPool.Get(GameEventId.SerializeTile)
                                         .With(EventParameters.Value, level);
            //FireEvent(tile, serializeTile.CreateEvent());
            tile.GetComponent<Tile>().SerializeTile(serializeTile);
            serializeTile.Release();
        }

        foreach (Room room in m_DungeonGenerator.Rooms)
            level.RoomData.Add(room);

        SaveSystem.Instance.SaveLevel(level, m_CurrentLevel);
    }

    void LoadOrCreateDungeon()
    {
        if (!m_DungeonLevelMap.ContainsKey(m_CurrentLevel))
        {
            DungeonGenerationResult loadedLevel = SaveSystem.Instance.LoadLevel(m_CurrentLevel);
            if (loadedLevel != null)
                m_DungeonLevelMap.Add(m_CurrentLevel, loadedLevel);
        }

        if (m_DungeonLevelMap.ContainsKey(m_CurrentLevel))
        {
            EntityFactory.ReloadTempBlueprints();
            DungeonGenerationResult dungeonLevel = m_DungeonLevelMap[m_CurrentLevel];
            foreach (var room in dungeonLevel.RoomData)
                m_DungeonGenerator.Rooms.Add(room);

            foreach(string wall in dungeonLevel.Walls)
            {
                IEntity entity = EntityFactory.ParseEntityData(wall);
                if (entity != null)
                {
                    entity.Start();
                    GameEvent getPoint = GameEventPool.Get(GameEventId.GetPoint)
                                            .With(EventParameters.Value, Point.InvalidPoint);
                    Point p = FireEvent(entity, getPoint).GetValue<Point>(EventParameters.Value);
                    if (p != Point.InvalidPoint)
                        Spawner.Spawn(entity, p);
                    getPoint.Release();
                }
            }

            foreach (string entityData in dungeonLevel.Entities)
            {
                IEntity entity = EntityFactory.ParseEntityData(entityData);
                if (entity != null)
                {
                    entity.Start();
                    GameEvent getPoint = GameEventPool.Get(GameEventId.GetPoint)
                                            .With(EventParameters.Value, Point.InvalidPoint);
                    Point p = FireEvent(entity, getPoint).GetValue<Point>(EventParameters.Value);
                    if (p != Point.InvalidPoint)
                        Spawner.Spawn(entity, p);
                    getPoint.Release();
                }
            }

            for (int i = 0; i < dungeonLevel.TilePoints.Count; i++)
            {
                FireEvent(m_Tiles[dungeonLevel.TilePoints[i]], 
                    GameEventPool.Get(GameEventId.SetHasBeenVisited)
                        .With(EventParameters.HasBeenVisited, dungeonLevel.TileHasBeenVisited[i])).Release();
            }
        }
        else
        {
            DungeonMetaData dmd = new DungeonMetaData($"{LevelMetaData.MetadataPath}/{m_CurrentLevel}.lvl");
            DungeonGenerationResult dr = m_DungeonGenerator.GenerateDungeon(dmd);
            using (new DiagnosticsTimer("Setting visited status"))
            {
                foreach (var point in m_Tiles.Keys)
                {
                    FireEvent(m_Tiles[point], GameEventPool.Get(GameEventId.SetHasBeenVisited)
                        .With(EventParameters.HasBeenVisited, false)).Release();
                }
            }
            m_DungeonLevelMap.Add(m_CurrentLevel, dr);
        }
    }


    void CleanTiles()
    {
        foreach (var tile in m_Tiles.Values)
            tile.GetComponent<Tile>().CleanTile();
            //FireEvent(tile, GameEventPool.Get(GameEventId.CleanTile));

        List<IEntity> entities = new List<IEntity>(m_EntityToPointMap.Keys);
        foreach (var entity in entities)
        {
            if (m_Players.Contains(entity))
                continue;
            if (!m_EntityToPointMap.ContainsKey(entity))
                continue;

            m_EntityToPointMap.Remove(entity);
            m_TimeProgression.RemoveEntity(entity);
        }
        m_DungeonGenerator.Clean();
        FireEvent(Self, GameEventPool.Get(GameEventId.CleanFoVData));
    }

    void SpawnPlayers()
    {
        string charactersPath = SaveSystem.kSaveDataPath + "/" + SaveSystem.Instance.CurrentSaveName + "/Blueprints/Characters";
#if UNITY_EDITOR
        if (!Directory.Exists(charactersPath))
        {
            for (int i = 0; i < 4; i++)
            {
                IEntity player = EntityFactory.CreateEntity("DwarfWarrior");
                FireEvent(Self, GameEventPool.Get(GameEventId.ConvertToPlayableCharacter).With(EventParameters.Entity, player.ID)).Release(); 
                Spawner.Spawn(player, m_DungeonGenerator.Rooms[0].GetValidPoint(null));

                player.CleanupComponents();

                FireEvent(player, GameEventPool.Get(GameEventId.InitFOV)).Release();
            }
        }

        else
#endif
        {
            foreach (var bp in Directory.EnumerateFiles(charactersPath, "*.bp"))
            {
                IEntity player = EntityFactory.CreateEntity(Path.GetFileNameWithoutExtension(bp));
                FireEvent(Self, GameEventPool.Get(GameEventId.ConvertToPlayableCharacter)
                    .With(EventParameters.Entity, player.ID)).Release();
                Spawner.Spawn(player, m_DungeonGenerator.Rooms[0].GetValidPoint(null));

                //if (i == 0)
                player.CleanupComponents();

                FireEvent(player, GameEventPool.Get(GameEventId.InitFOV)).Release();


                //var randomLineNumber = RecRandom.Instance.GetRandomValue(0, lines.Length - 1);
                //var line = lines[randomLineNumber];

                //GameEvent setName = GameEventPool.Get(GameEventId.SetName)
                //                        .With(EventParameters.Name, line);
                //FireEvent(player, setName.CreateEvent());

                //GameObject namePlate = GameObject.Instantiate(Resources.Load<GameObject>("UI/Nameplate"));
                //namePlate.GetComponent<NameplateMono>().Setup(player);
                //namePlate.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
                //namePlate.transform.SetAsFirstSibling();
            }
            Directory.Delete(charactersPath, true);
        }

        FireEvent(Self, GameEventPool.Get(GameEventId.ProgressTime)).Release();
    }

    //void SpawnPlayers()
    //{
    //    var lines = File.ReadAllLines($"{Application.streamingAssetsPath}/random_names.txt");

    //    for (int i = 0; i < 4; i++)
    //    {
    //        IEntity player = EntityFactory.CreateEntity("DwarfWarrior");
    //        FireEvent(Self, GameEventPool.Get(GameEventId.ConvertToPlayableCharacter, .With(EventParameters.Entity, player.ID)));
    //        Spawner.Spawn(player, m_DungeonGenerator.Rooms[0].GetValidPoint(null));

    //        if (i == 0)
    //            player.CleanupComponents();

    //        FireEvent(player, GameEventPool.Get(GameEventId.InitFOV));

            
    //        var randomLineNumber = RecRandom.Instance.GetRandomValue(0, lines.Length - 1);
    //        var line = lines[randomLineNumber];

    //        GameEvent setName = GameEventPool.Get(GameEventId.SetName)
    //                                .With(EventParameters.Name, line);
    //        FireEvent(player, setName.CreateEvent());

    //        GameObject namePlate = GameObject.Instantiate(Resources.Load<GameObject>("UI/Nameplate"));
    //        namePlate.GetComponent<NameplateMono>().Setup(player);
    //        namePlate.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
    //        namePlate.transform.SetAsFirstSibling();
    //    }

    //    FireEvent(Self, GameEventPool.Get(GameEventId.ProgressTime));
    //}

    void MovePlayersToCurrentFloor(bool movingDown)
    {
        foreach (var player in m_PlayerBlueprintCache)
        {
            var entity = EntityQuery.GetEntity(player);

            if (movingDown)
                Spawner.Spawn(entity, m_DungeonGenerator.Rooms[0].GetValidPoint(null));
            else
                Spawner.Spawn(entity, m_DungeonGenerator.Rooms[m_DungeonLevelMap[m_CurrentLevel].StairsDownRoomIndex].GetValidPoint(null));

            FireEvent(entity, GameEventPool.Get(GameEventId.RegisterPlayableCharacter)).Release();
            FireEvent(entity, GameEventPool.Get(GameEventId.InitFOV)).Release();
        }
        FireEvent(Self, GameEventPool.Get(GameEventId.UpdateWorldView)).Release();
        m_PlayerBlueprintCache.Clear();
    }

    void CreateTiles(Dictionary<Point, Actor> pointToTileMap)
    {
        EntityFactory.InitTempBlueprints();
        for (int i = 0; i < World.Services.MapColumns; i++)
        {
            for (int j = 0; j < World.Services.MapRows; j++)
            {
                CreateTile(i, j, m_Horizontal, m_Vertical, pointToTileMap);
            }
        }
    }

    protected void CreateTile(int x, int y, float screenHorizontal, float screenVertical, Dictionary<Point, Actor> pointToTileMap)
    {
        Point tilePoint = new Point(x, y);
        GameObject tile = UnityEngine.GameObject.Instantiate(m_TilePrefab);

        tile.transform.position = new Vector2(x - (screenHorizontal - 0.5f), y - (screenVertical - 0.5f));
        tile.transform.parent = UnityEngine.GameObject.Find("World").transform;
        m_GameObjectMap.Add(new Point(x, y), tile);

        Actor actor = new Actor("Tile");
        actor.AddComponent(new Tile(actor, new Point(x, y)));
        actor.AddComponent(new TileVisible(false));
        actor.AddComponent(new GraphicContainer("Textures/Environment/td_world_floor_cobble_b-120"));
        actor.AddComponent(new Renderer(tile.GetComponent<SpriteRenderer>()));
        actor.AddComponent(new Position(new Point(x, y)));
        actor.CleanupComponents();
        actor.Start();

        pointToTileMap.Add(new Point(x, y), actor);
        m_EntityToPointMap.Add(actor, new Point(x, y));
    }
}
