using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SocketIO;
using UnityEngine;

public struct NetworkIdContainer
{
    public string Id;
    public NetworkIdContainer(string id)
    {
        Id = id;
    }
}

public class EntityNetworkManager : GameService
{
    static string EnterWorld = "enterWorld";
    static string Spawn = "spawnPlayer";
    static string GetNetworkId = "getNetworkId";
    static string Ready = "ready";
    static string SyncWorldRequest = "syncWorldRequest";
    static string SyncWorld = "syncWorld";
    static string SyncCharacter = "syncCharacter";
    static string NetworkGameEvent = "gameEvent";
    static string Disconnect = "playerDisconnect";
    static string Despawn = "despawn";

    static string RequestDungeonLevel = "requestDungeonLevel";
    static string SyncDungeonFromClient = "syncDungeonFromClient";
    static string ServerRecievedDungeonSync = "serverRecievedDungeonSync";
    static string DungeonSyncComplete = "dungeonSyncComplete";

    static SocketIOComponent socket;

    public string NetworkId;
    public bool IsHost;
    public bool IsConnected;

    Dictionary<string, IEntity> m_NetworkIdToEntityMap = new Dictionary<string, IEntity>();

    public Action SyncWorldCompleted;
    public Action DungeonLevelSynced;
    public Action LocalPlayerSpawned;

    public EntityNetworkManager(SocketIOComponent sock)
    {
        if (sock != null)
        {
            socket = sock;
            socket.SetupSocket();
            OnConnect();
        }
    }

    void OnConnect()
    {
        socket.On(GetNetworkId, OnGetNetworkId);
        socket.On(Spawn, OnSpawnPlayer);
        socket.On(Ready, OnReady);
        socket.On(SyncWorldRequest, SyncWorldRequested);
        socket.On(SyncWorld, SyncWorldData);
        socket.On(SyncCharacter, OnSyncCharacterData);
        socket.On(NetworkGameEvent, OnGameEvent);
        socket.On(Disconnect, OnDisconnect);
        socket.On(Despawn, OnDespawn);
        
        
        socket.On(RequestDungeonLevel, OnDungeonLevelRequested);
        socket.On(SyncDungeonFromClient, OnSyncDungeonLevel);
        socket.On(ServerRecievedDungeonSync, OnRecievedDungeonLevel);
        socket.On(DungeonSyncComplete, OnDungeonLevelSyncComplete);
    }

    string GetNetworkIdFromSocketEvent(SocketIOEvent e)
    {
        return e.data["NetworkId"].ToString().Trim(new char[] { '\\', '\"' });
    }

    void OnDisconnect(SocketIOEvent e)
    {
        string disconnectedPlayer = GetNetworkIdFromSocketEvent(e);
        IEntity entity = m_NetworkIdToEntityMap[disconnectedPlayer];
        Spawner.Despawn(entity);
        Services.WorldUIService.UnRegisterPlayableCharacter(entity.ID);
        m_NetworkIdToEntityMap.Remove(disconnectedPlayer);
        Services.WorldUpdateService.UpdateWorldView();
    }

    public void DespawnEntity(IEntity deSpawn)
    {
        //socket.Emit(Despawn, CreateJSONObject(deSpawn.ID));
    }

    void OnDespawn(SocketIOEvent e)
    {
        //string id = e.data["ID"].ToString();
        //Spawner.Despawn(Services.EntityMapService.GetEntity(id));
        //Services.WorldUpdateService.UpdateWorldView();
    }

    //A client has requested level data from the server
    void OnDungeonLevelRequested(SocketIOEvent e)
    {
        if(IsHost)
        {
            Services.SaveAndLoadService.Save();
            //The server puts out a request to all clients
            socket.Emit(SyncDungeonFromClient, e.data);
        }
    }

    //For a non-host client to send its dungeon level data to the server
    void OnSyncDungeonLevel(SocketIOEvent e)
    {
        string requestingId = JsonUtility.FromJson<NetworkIdContainer>(e.data["RequestingClientId"].ToString()).Id;

        if(requestingId != NetworkId)
                Services.SaveAndLoadService.Save();

        if (!IsHost)
        {
            string dungeonLevel = e.data["DungeonLevel"].ToString();
            string levelDataFilePath = $"{Services.SaveAndLoadService.LoadPath}/{dungeonLevel}/data.dat";
            List<string> levelData = new List<string>();
            List<string> dungeonLevels = new List<string>();
            List<string> dateTimes = new List<string>();
            if (File.Exists(levelDataFilePath))
            {
                dungeonLevels.Add(dungeonLevel);
                dateTimes.Add(File.GetLastWriteTime(levelDataFilePath).ToString());
                levelData.Add(File.ReadAllText(levelDataFilePath));
            }
            else
            {
                levelData.Add(string.Empty);
                dateTimes.Add(string.Empty);
                dungeonLevels.Add(dungeonLevel);
            }

            //Send a sync back to the server with the data
            NetworkDungeonData ndd = new NetworkDungeonData(null, levelData, dungeonLevels, dateTimes, GetAllLocalTempBlueprints());
            socket.Emit(ServerRecievedDungeonSync, CreateJSONObject(ndd));
        }
    }

    int m_RequestsReceived = 0;
    void OnRecievedDungeonLevel(SocketIOEvent e)
    {
        if(IsHost)
        {
            var ndd = JsonUtility.FromJson<NetworkDungeonData>(e.data.ToString());
            Services.SaveAndLoadService.UpdateSaveFromNetwork(ndd);
            EntityFactory.ReloadTempBlueprints();
            m_RequestsReceived++;
            if(m_RequestsReceived == m_NetworkIdToEntityMap.Count)
            {
                m_RequestsReceived = 0;

                List<string> levelData = new List<string>();
                List<string> writeTimes = new List<string>();
                foreach(var dl in ndd.LevelsToUpdate)
                {
                    string saveData = $"{Services.SaveAndLoadService.LoadPath}/{dl}/data.dat";
                    if (File.Exists(saveData))
                    {
                        levelData.Add(File.ReadAllText(saveData));
                        writeTimes.Add(File.GetLastWriteTime(saveData).ToString());
                    }
                    else
                    {
                        levelData.Add(string.Empty);
                        writeTimes.Add(string.Empty);
                    }
                }

                NetworkDungeonData newNDD = new NetworkDungeonData(null, levelData, ndd.LevelsToUpdate, writeTimes, GetAllLocalTempBlueprints());

                socket.Emit(DungeonSyncComplete, CreateJSONObject(newNDD));
                OnDungeonLevelSyncComplete(null);
            }
        }
    }

    /// <summary>
    /// For use when trying to load a specifc level
    /// </summary>
    /// <param name="level"></param>
    public void RequestDungeonLevelFromServer(int level)
    {
        var obj = JSONObject.Create();
        
        obj["DungeonLevel"] = JSONObject.Create(level);
        obj["RequestingClientId"] = CreateJSONObject(new NetworkIdContainer(NetworkId));

        if(!IsHost)
            socket.Emit(RequestDungeonLevel, obj);
        else
        {
            //Services.SaveAndLoadService.Save();
            if (m_NetworkIdToEntityMap.Count > 0)
                socket.Emit(SyncDungeonFromClient, obj);
            else
                OnDungeonLevelSyncComplete(null);
        }
    }

    void OnDungeonLevelSyncComplete(SocketIOEvent e)
    {
        if (!IsHost)
        {
            var ndd = JsonUtility.FromJson<NetworkDungeonData>(e.data.ToString());
            Services.SaveAndLoadService.UpdateSaveFromNetwork(ndd);
        }

        DungeonLevelSynced?.Invoke();
        DungeonLevelSynced = null;
    }

    void OnGameEvent(SocketIOEvent e)
    {
        var dungeonLevel = int.Parse(e.data["DungeonLevel"].ToString());
        if (dungeonLevel != Services.DungeonService.GetCurrentLevel())
            return;
        GameEventSerializable ges = JsonUtility.FromJson<GameEventSerializable>(e.data["Event"].ToString());
        GameEvent ge = ges.CreateGameEvent();

        if (ge.ID == GameEventId.Despawn)
        {
            if (Point.TryParse(ges.TargetEntityId, out Point result))
            {
                EntityType eType = (EntityType)Enum.Parse(typeof(EntityType), ge.GetValue<string>(EventParameters.EntityType));
                ge.Paramters[EventParameters.EntityType] = eType;
                IEntity source = Services.EntityMapService.GetEntity(ge.GetValue<string>(EventParameters.Entity));
                Services.FOVService.FoVRecalculated(source, new List<Point>());
                m_Tiles[result].Despawn(ge);
            }
            else
            {
                ge.Release();
                return;
            }
        }
        else
        {
            IEntity entity = Services.EntityMapService.GetEntity(ges.TargetEntityId);
            entity.FireEvent(ge);
            ge.Release();
        }
        Services.WorldUpdateService.UpdateWorldView();
    }

    void OnSyncCharacterData(SocketIOEvent e)
    {
        {
            string data = e.data["EntityData"].ToString()
            .Trim(new char[] { '\\', '\"' })
            .Replace("\\t", "\t")
            .Replace("\\n", "\n")
            .Replace("\\r", "\r");

            var newConnectionNDD = JsonUtility.FromJson<NetworkEntityData>(e.data.ToString());
            if (newConnectionNDD.CurrentLevel != m_CurrentLevel)
                return;

            foreach (var bp in newConnectionNDD.Blueprints)
            {
                string name = EntityFactory.GetEntityNameFromBlueprintFormatting(bp);
                EntityFactory.CreateTemporaryBlueprint(name.Split(',')[1], bp);
            }

            EntityFactory.ReloadTempBlueprints();
            IEntity networkEntity = EntityFactory.ParseEntityData(data);

            if (networkEntity.GetComponent<NetworkId>().ID != NetworkId)
            {
                if(!networkEntity.HasComponent(typeof(NetworkController)))
                    networkEntity.AddComponent(new NetworkController());
                
                networkEntity.RemoveComponent(typeof(RegisterPlayableCharacter));
                networkEntity.RemoveComponent(typeof(RegisterWithTimeSystem));
                Services.WorldUIService.RegisterPlayableCharacter(networkEntity.ID);

                if (!m_NetworkIdToEntityMap.ContainsKey(networkEntity.GetComponent<NetworkId>().ID))
                    m_NetworkIdToEntityMap.Add(networkEntity.GetComponent<NetworkId>().ID, networkEntity);
            }

            networkEntity.CleanupComponents();
            Spawner.Spawn(networkEntity, networkEntity.GetComponent<Position>().PositionPoint);
            
            Services.WorldUpdateService.UpdateWorldView();
            if(networkEntity.GetComponent<NetworkId>().ID == NetworkId)
            {
                LocalPlayerSpawned?.Invoke();
                LocalPlayerSpawned = null;
            }
        }
    }

    void SyncWorldData(SocketIOEvent e)
    {
        if (!IsHost)
        {
            var ndd = JsonUtility.FromJson<NetworkDungeonData>(e.data.ToString());
            Services.SaveAndLoadService.UpdateSaveFromNetwork(ndd);
            EntityFactory.ReloadTempBlueprints();

            Services.WorldUpdateService.StopTime = false;
            SyncWorldCompleted?.Invoke();
        }
    }

    public void EmitEvent(GameEventSerializable sge)
    {
        var obj = JSONObject.Create();

        obj["DungeonLevel"] = JSONObject.Create(Services.DungeonService.GetCurrentLevel());
        obj["Event"] = JSONObject.Create(JsonUtility.ToJson(sge));

        socket.Emit(NetworkGameEvent, obj);
    }

    void SyncWorldRequested(SocketIOEvent e)
    {
        if (IsHost)
        {
            Services.WorldUpdateService.StopTime = true;
            Services.SaveAndLoadService.Save();

            var newConnectionNDD = JsonUtility.FromJson<NetworkDungeonData>(e.data.ToString());
            foreach (var bp in newConnectionNDD.TempBlueprints)
            {
                string name = EntityFactory.GetEntityNameFromBlueprintFormatting(bp);
                EntityFactory.CreateTemporaryBlueprint(name.Split(',')[1], bp);
            }

            string savePath = Services.SaveAndLoadService.LoadPath;
            string saveFile = File.ReadAllText(Services.SaveAndLoadService.CurrentSavePath);
            List<string> levelDatas = new List<string>();
            List<string> bluePrints = new List<string>();
            List<string> levelsToUpdate = new List<string>();
            List<string> dateTimes = new List<string>();

            int i = 1;
            while (Directory.Exists($"{savePath}/{i}"))
            {
                string path = $"{savePath}/{i}/data.dat";
                levelDatas.Add(File.ReadAllText(path));
                levelsToUpdate.Add(i.ToString());
                dateTimes.Add(File.GetLastWriteTime(path).ToString());
                i++;
            }

            if (Directory.Exists($"{savePath}/Blueprints"))
            {
                foreach (var bp in Directory.EnumerateFiles($"{savePath}/Blueprints"))
                    bluePrints.Add(File.ReadAllText(bp));
            }

            NetworkDungeonData ndd = new NetworkDungeonData(saveFile, levelDatas, levelsToUpdate, dateTimes, bluePrints);
            socket.Emit(SyncWorld, CreateJSONObject(ndd));
            Services.WorldUpdateService.StopTime = false;
        }
    }

    List<string> GetAllLocalTempBlueprints()
    {
        string savePath = Services.SaveAndLoadService.LoadPath;
        List<string> bluePrints = new List<string>();
        if (Directory.Exists($"{savePath}/Blueprints"))
        {
            foreach (var bp in Directory.EnumerateFiles($"{savePath}/Blueprints"))
                bluePrints.Add(File.ReadAllText(bp));
        }
        return bluePrints;
    }

    void OnReady(SocketIOEvent e)
    {
        if (!IsConnected)
        {
            Debug.Log("Ready called");
            socket.Emit(EnterWorld, CreateJSONObject(new NetworkIdData()));
            IsConnected = true;
        }
    }

    void OnSpawnPlayer(SocketIOEvent e)
    {
        if (IsHost)
        {
            string data = e.data["EntityData"].ToString()
            .Trim(new char[] { '\\', '\"' })
            .Replace("\\t", "\t")
            .Replace("\\n", "\n")
            .Replace("\\r", "\r");

            var newConnectionNED = JsonUtility.FromJson<NetworkEntityData>(e.data.ToString());
            foreach (var bp in newConnectionNED.Blueprints)
            {
                string name = EntityFactory.GetEntityNameFromBlueprintFormatting(bp);
                EntityFactory.CreateTemporaryBlueprint(name.Split(',')[1], bp);
            }

            EntityFactory.ReloadTempBlueprints();

            IEntity networkEntity = EntityFactory.ParseEntityData(data);
            Services.PlayerManagerService.ConvertToPlayableEntity(networkEntity);
            networkEntity.CleanupComponents();

            GameEvent getPosition = GameEventPool.Get(GameEventId.GetPoint)
                                .With(EventParameters.Value, Point.InvalidPoint);
            Point p = networkEntity.FireEvent(getPosition).GetValue<Point>(EventParameters.Value);
            getPosition.Release();

            networkEntity.GetComponent<Position>().PositionPoint = p == new Point(0, 0) ? Services.DungeonService.DungeonGenerator.Rooms[0].GetValidPoint() : p;
            networkEntity.CleanupComponents();

            var serializedData = networkEntity.Serialize();

            string savePath = Services.SaveAndLoadService.LoadPath;
            if (Directory.Exists($"{savePath}/Blueprints"))
            {
                foreach (var bp in Directory.EnumerateFiles($"{savePath}/Blueprints"))
                    newConnectionNED.Blueprints.Add(File.ReadAllText(bp));
            }
            newConnectionNED.Blueprints = newConnectionNED.Blueprints.Distinct().ToList();

            socket.Emit(SyncCharacter, CreateJSONObject(new NetworkEntityData(serializedData, newConnectionNED.Blueprints, newConnectionNED.CurrentLevel)));

            Services.WorldUpdateService.StopTime = false;
        }
    }

    void OnGetNetworkId(SocketIOEvent e)
    {
        NetworkId = e.data["NetworkId"].ToString();
        NetworkId = NetworkId.Trim(new char[] { '\\', '\"' });

        IsHost = bool.Parse(e.data["isHost"].ToString());

        if (IsHost)
        {
            Services.DungeonService.GenerateDungeon(true, Services.SaveAndLoadService.LoadPath);
            foreach (var player in Services.PlayerManagerService.GetPlayerEntitiesToSpawn())
                Services.NetworkService.SpawnPlayer(player);
        }
        else
        {
            SyncWorldCompleted += SpawnPlayers;
            SyncWorldWithHost();
        }
    }

    public void ConvertToNetworkedPlayer(IEntity player)
    {
        player.RemoveComponent(typeof(InputControllerBase));
        player.RemoveComponent(typeof(RegisterPlayableCharacter));
        player.RemoveComponent(typeof(RegisterWithTimeSystem));
        player.AddComponent(new NetworkController());
        player.CleanupComponents();
    }

    void SpawnPlayers()
    {
        Services.SaveAndLoadService.Load(Services.SaveAndLoadService.CurrentSavePath, false);

        List<IEntity> remotePlayers = new List<IEntity>();
        foreach (var player in m_Players)
        {
            if (player.GetComponent<NetworkId>().ID != NetworkId)
            {
                ConvertToNetworkedPlayer(player);
                Services.WorldUIService.RegisterPlayableCharacter(player.ID);
                remotePlayers.Add(player);
            }
        }

        foreach (var rp in remotePlayers)
        {
            m_TimeProgression.RemoveEntity(rp);
            m_Players.Remove(rp);
            if (m_ActivePlayer.Value == rp)
                m_ActivePlayer = null;
        }

        foreach (var player in Services.PlayerManagerService.GetPlayerEntitiesToSpawn())
            SpawnPlayer(player);

        SyncWorldCompleted -= SpawnPlayers;
    }

    public void SpawnPlayer(IEntity e)
    {
        if(!e.HasComponent(typeof(NetworkId)))
            e.AddComponent(new NetworkId(NetworkId));
        
        string data = e.Serialize();
        List<string> bluePrints = new List<string>();

        string savePath = Services.SaveAndLoadService.LoadPath;
        if (Directory.Exists($"{savePath}/Blueprints"))
        {
            foreach (var bp in Directory.EnumerateFiles($"{savePath}/Blueprints"))
                bluePrints.Add(File.ReadAllText(bp));
        }

        socket.Emit(Spawn, CreateJSONObject(new NetworkEntityData(data, bluePrints, m_CurrentLevel)));
    }

    public void SyncWorldWithHost()
    {
        Services.WorldUpdateService.StopTime = true;
        Services.SaveAndLoadService.Save();

        string savePath = Services.SaveAndLoadService.LoadPath;

        List<string> bluePrints = new List<string>();
        List<string> levelDatas = new List<string>();
        List<string> levelsToUpdate = new List<string>();
        List<string> dateTimes = new List<string>();

        int i = 1;
        while (Directory.Exists($"{savePath}/{i}"))
        {
            string path = $"{savePath}/{i}/data.dat";
            levelDatas.Add(File.ReadAllText(path));
            levelsToUpdate.Add(i.ToString());
            dateTimes.Add(File.GetLastWriteTime(path).ToString());
            i++;
        }

        if (Directory.Exists($"{savePath}/Blueprints"))
        {
            foreach (var bp in Directory.EnumerateFiles($"{savePath}/Blueprints"))
                bluePrints.Add(File.ReadAllText(bp));
        }

        socket.Emit(SyncWorldRequest, CreateJSONObject(new NetworkDungeonData(null, levelDatas, levelsToUpdate, dateTimes, bluePrints)));
    }

    JSONObject CreateJSONObject(object obj)
    {
        return JSONObject.Create(JsonUtility.ToJson(obj).ToString());
    }
}
