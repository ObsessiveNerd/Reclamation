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
    public string WallType;
    public int AverageMosterCR;

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
                        case LevelMetaData.WallType:
                            WallType = keyValue[1];
                            break;
                        case LevelMetaData.AverageMosterCR:
                            AverageMosterCR = int.Parse(keyValue[1]);
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

    public string GetRandomLetter()
    {
        var number = RecRandom.Instance.GetRandomValue(0, 5);
        string value = "a";
        switch(number)
        {
            case 0:
                break;
            case 1:
                value = "b";
                break;
            case 2:
                value = "c";
                break;
            case 3:
                value = "d";
                break;
            //case 4:
            //    value = "e";
            //    break;
            //case 5:
            //    value = "f";
            //    break;
        }
        return value;
    }
}

public class DungeonManager : GameService
{
    GameObject m_TilePrefab;
    public IDungeonGenerator DungeonGenerator;
    int m_Vertical, m_Horizontal;
    public bool IsReady = false;

    public DungeonManager(int seed, GameObject tilePrefab, int rows, int columns)
    {
        m_Seed = seed;
        m_TilePrefab = tilePrefab;
        m_Vertical = rows;
        m_Horizontal = columns;
    }

    public void GenerateDungeon(bool startNew, string loadPath)
    {
        if (startNew)
        {
            using (new DiagnosticsTimer("Start World"))
                GenerateDungeon(1, true);

            if (!Services.NetworkService.IsConnected)
            {
                foreach (var player in Services.PlayerManagerService.GetPlayerEntitiesToSpawn())
                {
                    Services.PlayerManagerService.ConvertToPlayableEntity(player);
                    Spawner.Spawn(player, Services.DungeonService.DungeonGenerator.Rooms[0].GetValidPoint(null));
                    player.CleanupComponents();

                    player.FireEvent(GameEventPool.Get(GameEventId.InitFOV)).Release();
                }
            }
            using (new DiagnosticsTimer("Update world view"))
                Services.WorldUpdateService.UpdateWorldView();
        }
        else
        {
            Services.SaveAndLoadService.Load($"{loadPath}/data.save");
        }
    }

    public void GenerateDungeon(int level, bool newGame)
    {
        Debug.Log("Start world called");

        Clean();
        CreateTiles(m_Horizontal, m_Vertical);
        m_CurrentLevel = level;
        DungeonGenerator = new BasicDungeonGenerator(m_Horizontal, m_Vertical);

        LoadOrCreateDungeon();

        Services.SaveAndLoadService.Save();
        Services.CameraService.UpdateCamera();
        m_TimeProgression.Resume();
        IsReady = true;
    }

    public void MoveUp()
    {
        m_TimeProgression.SetPostFrameCallback(() =>
            {
                CachePlayers();
                Services.SaveAndLoadService.Save();
                CleanTiles();
                m_CurrentLevel--;
                if(Services.NetworkService.IsConnected)
                {
                    Services.NetworkService.DungeonLevelSynced += () =>
                    {
                        Services.WorldUpdateService.StopTime = true;
                        LoadOrCreateDungeon();
                        Services.NetworkService.LocalPlayerSpawned += () =>
                        {
                            Services.WorldUpdateService.UpdateWorldView();
                            Services.CameraService.UpdateCamera();
                            Services.WorldUpdateService.StopTime = false;
                        };
                        MovePlayersToCurrentFloor(false);
                    };
                    Services.NetworkService.RequestDungeonLevelFromServer(m_CurrentLevel);
                }
                else
                {
                    LoadOrCreateDungeon();
                    MovePlayersToCurrentFloor(false);
                    Services.CameraService.UpdateCamera();
                }
            });
    }

    public void MoveDown()
    {
        m_TimeProgression.SetPostFrameCallback(() =>
            {
                CachePlayers();
                Services.SaveAndLoadService.Save();
                CleanTiles();
                m_CurrentLevel++;

                if(Services.NetworkService.IsConnected)
                {
                    Services.NetworkService.DungeonLevelSynced += () =>
                    {
                        Services.WorldUpdateService.StopTime = true;
                        LoadOrCreateDungeon();
                        Services.NetworkService.LocalPlayerSpawned += () =>
                        {
                            Services.WorldUpdateService.UpdateWorldView();
                            Services.CameraService.UpdateCamera();
                            Services.WorldUpdateService.StopTime = false;
                        };
                        MovePlayersToCurrentFloor(true);
                    };
                    Services.NetworkService.RequestDungeonLevelFromServer(m_CurrentLevel);
                }
                else
                {
                    LoadOrCreateDungeon();
                    MovePlayersToCurrentFloor(true);
                    Services.CameraService.UpdateCamera();
                }
            });
    }
    public int GetCurrentLevel()
    {
        return m_CurrentLevel;
    }

    public void SetCurrentLevel(int level)
    {
        m_CurrentLevel = level;
    }

    public Point GetRandomValidPoint()
    {
        if (DungeonGenerator == null)
            return new Point(1, 1);
        else
        {
            int roomIndex = RecRandom.Instance.GetRandomValue(0, DungeonGenerator.Rooms.Count - 1);
            return DungeonGenerator.Rooms[roomIndex].GetValidPoint(null);
        }
    }

    public List<Point> GetValidPointsAround(Point startPoint, int range)
    {
        if (DungeonGenerator != null)
        {
            foreach(var room in DungeonGenerator.Rooms)
            {
                if (room.ContainsPoint(startPoint))
                    return room.GetValidPointsAround(startPoint, range);
            }
        }

        return new List<Point>();
    }

    public Point GetRandomValidPointInSameRoom(Point startPoint)
    {
        if (DungeonGenerator == null)
            return new Point(1, 1);
        else
        {
            foreach(var room in DungeonGenerator.Rooms)
            {
                if (room.ContainsPoint(startPoint))
                    return room.GetValidPoint();
            }
        }

        return Point.InvalidPoint;
    }

    public void AddValidPoint(HashSet<Point> validPoints)
    {
        foreach (Point p in validPoints)
            m_ValidDungeonPoints.Add(p);
    }

    List<string> m_PlayerBlueprintCache = new List<string>();
    void CachePlayers()
    {
        m_PlayerBlueprintCache.Clear();
        foreach (var player in m_Players)
        {
            m_PlayerBlueprintCache.Add(player.ID);
            Services.FOVService.UnRegisterPlayer(player);
            Spawner.Despawn(player);
        }
    }

    void LoadOrCreateDungeon()
    {
        if (!m_DungeonLevelMap.ContainsKey(m_CurrentLevel) || Services.NetworkService.IsConnected)
        {
            DungeonGenerationResult loadedLevel = Services.SaveAndLoadService.LoadLevel(m_CurrentLevel);
            if (loadedLevel != null)
                if (m_DungeonLevelMap.ContainsKey(m_CurrentLevel))
                    m_DungeonLevelMap[m_CurrentLevel] = loadedLevel;
                else
                    m_DungeonLevelMap.Add(m_CurrentLevel, loadedLevel);
        }

        if (m_DungeonLevelMap.ContainsKey(m_CurrentLevel))
        {
            EntityFactory.ReloadTempBlueprints();
            DungeonGenerationResult dungeonLevel = m_DungeonLevelMap[m_CurrentLevel];
            foreach (var room in dungeonLevel.RoomData)
                DungeonGenerator.Rooms.Add(room);

            foreach(string wall in dungeonLevel.Walls)
            {
                IEntity entity = EntityFactory.ParseEntityData(wall);
                if (entity != null)
                {
                    entity.Start();
                    GameEvent getPoint = GameEventPool.Get(GameEventId.GetPoint)
                                            .With(EventParameter.Value, Point.InvalidPoint);
                    Point p = FireEvent(entity, getPoint).GetValue<Point>(EventParameter.Value);
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
                                            .With(EventParameter.Value, Point.InvalidPoint);
                    Point p = FireEvent(entity, getPoint).GetValue<Point>(EventParameter.Value);
                    if (p != Point.InvalidPoint)
                    {
                        if (Services.NetworkService.IsConnected)
                        {
                            if (entity.HasComponent(typeof(PlayerInputController)))
                            { 
                                Services.NetworkService.ConvertToNetworkedPlayer(entity);
                                Services.WorldUIService.RegisterPlayableCharacter(entity.ID);
                            }
                        }
                         
                        Spawner.Spawn(entity, p);
                    }
                    getPoint.Release();
                }
            }

            for (int i = 0; i < dungeonLevel.TilePoints.Count; i++)
            {
                FireEvent(m_TileEntity[dungeonLevel.TilePoints[i]], 
                    GameEventPool.Get(GameEventId.SetHasBeenVisited)
                        .With(EventParameter.HasBeenVisited, dungeonLevel.TileHasBeenVisited[i])).Release();
            }

            DungeonMetaData dmd = new DungeonMetaData($"{LevelMetaData.MetadataPath}/{m_CurrentLevel}.lvl");
            foreach(var tile in m_TileEntity.Values)
            {
                GameEvent setSprite = GameEventPool.Get(GameEventId.SetSprite)
                                        .With(EventParameter.Path, dmd.TileType + dmd.GetRandomLetter());
                FireEvent(tile, setSprite);
                setSprite.Release();
            }
        }
        else
        {
            DungeonMetaData dmd = new DungeonMetaData($"{LevelMetaData.MetadataPath}/{m_CurrentLevel}.lvl");
            DungeonGenerationResult dr = DungeonGenerator.GenerateDungeon(dmd, m_CurrentLevel == 20);

            foreach(var tile in m_TileEntity.Values)
            {
                GameEvent setSprite = GameEventPool.Get(GameEventId.SetSprite)
                                        .With(EventParameter.Path, dmd.TileType + dmd.GetRandomLetter());
                FireEvent(tile, setSprite);
                setSprite.Release();
            }

            using (new DiagnosticsTimer("Setting visited status"))
            {
                foreach (var point in m_Tiles.Keys)
                {
                    FireEvent(m_TileEntity[point], GameEventPool.Get(GameEventId.SetHasBeenVisited)
                        .With(EventParameter.HasBeenVisited, false)).Release();
                }
            }
            m_DungeonLevelMap.Add(m_CurrentLevel, dr);
        }
    }

    void CleanTiles()
    {
        foreach (var tile in m_Tiles.Values)
            tile.CleanTile();
            //FireEvent(tile, GameEventPool.Get(GameEventId.CleanTile));

        List<string> entities = new List<string>(m_EntityToPointMap.Keys);
        foreach (var entity in entities)
        {
            if (m_Players.Contains(m_EntityIdToEntityMap[entity]))
                continue;
            if (!m_EntityToPointMap.ContainsKey(entity) || m_EntityIdToEntityMap[entity].HasComponent(typeof(Tile)))
                continue;

            m_EntityToPointMap.Remove(entity);
            m_TimeProgression.RemoveEntity(m_EntityIdToEntityMap[entity]);
        }
        DungeonGenerator.Clean();
        Services.FOVService.CleanFoVData();
    }

    void MovePlayersToCurrentFloor(bool movingDown)
    {
        foreach (var player in m_PlayerBlueprintCache)
        {
            var entity = EntityQuery.GetEntity(player);

            if (Services.NetworkService.IsConnected)
            {
                Point requestedPoint = new Point(0, 0);
                if (movingDown)
                    requestedPoint = DungeonGenerator.Rooms[0].GetValidPoint(null);
                else
                    requestedPoint = DungeonGenerator.Rooms[m_DungeonLevelMap[m_CurrentLevel].StairsDownRoomIndex].GetValidPoint(null);
                entity.GetComponent<Position>().PositionPoint = requestedPoint;

                Services.NetworkService.SpawnPlayer(entity);
            }
            else
            {
                if (movingDown)
                    Spawner.Spawn(entity, DungeonGenerator.Rooms[0].GetValidPoint(null));
                else
                    Spawner.Spawn(entity, DungeonGenerator.Rooms[m_DungeonLevelMap[m_CurrentLevel].StairsDownRoomIndex].GetValidPoint(null));

                FireEvent(entity, GameEventPool.Get(GameEventId.RegisterPlayableCharacter)).Release();
                FireEvent(entity, GameEventPool.Get(GameEventId.InitFOV)).Release();
            }
        }
        if(!Services.NetworkService.IsConnected)
            Services.WorldUpdateService.UpdateWorldView();
        m_PlayerBlueprintCache.Clear();
    }

    void CreateTiles(int rows, int columns)
    {
        EntityFactory.InitTempBlueprints();
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                CreateTile(i, j, m_Horizontal, m_Vertical);
            }
        }
    }

    protected void CreateTile(int x, int y, float screenHorizontal, float screenVertical)
    {
        Point tilePoint = new Point(x, y);
        GameObject tile = UnityEngine.GameObject.Instantiate(m_TilePrefab);

        tile.transform.position = new Vector2(x - (screenHorizontal - 0.5f), y - (screenVertical - 0.5f));
        tile.transform.parent = UnityEngine.GameObject.Find("World").transform;
        m_GameObjectMap.Add(new Point(x, y), tile);

        Actor actor = new Actor("Tile");
        Tile t = new Tile(actor, new Point(x, y));
        actor.AddComponent(t);
        actor.AddComponent(new TileVisible(false));
        actor.AddComponent(new GraphicContainer("Textures/Sprites/Environment/td_world_floor_cobble_b-120"));
        actor.AddComponent(new Renderer(tile.GetComponent<SpriteRenderer>()));
        actor.AddComponent(new Position(new Point(x, y)));
        actor.CleanupComponents();
        actor.Start();

        m_Tiles.Add(new Point(x, y), t);
        m_TileEntity.Add(new Point(x, y), actor);
        m_EntityToPointMap.Add(actor.ID, new Point(x, y));
    }
}
