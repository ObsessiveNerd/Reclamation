using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SocketIO;
using UnityEngine;

public class EntityNetworkManager : GameService
{
    static string ProcessGameEvent = "processGameEvent";
    static string EnterWorld = "enterWorld";
    static string Spawn = "spawnPlayer";
    static string GetNetworkId = "getNetworkId";
    static string Ready = "ready";
    static string SyncWorldRequest = "syncWorldRequest";
    static string SyncWorld = "syncWorld";
    static string SyncCharacter = "syncCharacter";

    static SocketIOComponent socket;

    public string NetworkId;
    public bool IsHost;
    public bool IsConnected;

    Dictionary<string, string> m_NetworkdIdToPlayerIdMap = new Dictionary<string, string>();

    Action SyncWorldCompleted;

    public EntityNetworkManager(SocketIOComponent sock)
    {
        socket = sock;
        socket.SetupSocket();
        OnConnect();
    }

    void OnConnect()
    {
        //socket.On(EnterWorld, OnEnterWorld);
        socket.On(GetNetworkId, OnGetNetworkId);
        socket.On(Spawn, OnSpawnPlayer);
        socket.On(ProcessGameEvent, OnProcessGameEvent);
        socket.On(Ready, OnReady);
        socket.On(SyncWorldRequest, SyncWorldRequested);
        socket.On(SyncWorld, SyncWorldData);
        socket.On(SyncCharacter, OnSyncCharacterData);
    }

    void OnSyncCharacterData(SocketIOEvent e)
    {
        //if(!IsHost)
        {
            string data = e.data["EntityData"].ToString()
            .Trim(new char[] { '\\', '\"' })
            .Replace("\\t", "\t")
            .Replace("\\n", "\n")
            .Replace("\\r", "\r");

            var newConnectionNDD = JsonUtility.FromJson<NetworkEntityData>(e.data.ToString());
            foreach (var bp in newConnectionNDD.Blueprints)
            {
                string name = EntityFactory.GetEntityNameFromBlueprintFormatting(bp);
                EntityFactory.CreateTemporaryBlueprint(name.Split(',')[1], bp);
            }

            EntityFactory.ReloadTempBlueprints();
            IEntity networkEntity = EntityFactory.ParseEntityData(data);

            if (networkEntity.GetComponent<NetworkId>().ID != NetworkId)
            {
                networkEntity.AddComponent(new NetworkController());
                networkEntity.RemoveComponent(typeof(RegisterPlayableCharacter));
                networkEntity.RemoveComponent(typeof(RegisterWithTimeSystem));
                Services.WorldUIService.RegisterPlayableCharacter(networkEntity.ID);
            }

            networkEntity.CleanupComponents();
            Spawner.Spawn(networkEntity, networkEntity.GetComponent<Position>().PositionPoint);

            networkEntity.FireEvent(GameEventPool.Get(GameEventId.InitFOV)).Release();
            Services.WorldUpdateService.UpdateWorldView();
        }
    }

    void SyncWorldData(SocketIOEvent e)
    {
        if (!IsHost)
        {
            Services.SaveAndLoadService.MoveSaveData(Hash128.Compute(socket.url).ToString());

            var ndd = JsonUtility.FromJson<NetworkDungeonData>(e.data.ToString());
            Services.SaveAndLoadService.UpdateSaveFromNetwork(ndd);
            EntityFactory.ReloadTempBlueprints();
            Services.SaveAndLoadService.Load(Services.SaveAndLoadService.CurrentSavePath, false);

            List<IEntity> remotePlayers = new List<IEntity>();
            foreach (var player in m_Players)
            {
                if (player.GetComponent<NetworkId>().ID != NetworkId)
                {
                    player.RemoveComponent(typeof(InputControllerBase));
                    player.RemoveComponent(typeof(RegisterPlayableCharacter));
                    player.RemoveComponent(typeof(RegisterWithTimeSystem));
                    player.AddComponent(new NetworkController());
                    player.CleanupComponents();
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

            Services.WorldUpdateService.StopTime = false;
            SyncWorldCompleted?.Invoke();
        }
    }

    void SyncWorldRequested(SocketIOEvent e)
    {
        if (IsHost)
        {
            Services.SaveAndLoadService.Save();
            Services.WorldUpdateService.StopTime = true;

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

            int i = 1;
            while (Directory.Exists($"{savePath}/{i}"))
            {
                string path = $"{savePath}/{i}/data.dat";
                levelDatas.Add(File.ReadAllText(path));
                i++;
            }

            if (Directory.Exists($"{savePath}/Blueprints"))
            {
                foreach (var bp in Directory.EnumerateFiles($"{savePath}/Blueprints"))
                    bluePrints.Add(File.ReadAllText(bp));
            }

            NetworkDungeonData ndd = new NetworkDungeonData(saveFile, levelDatas, bluePrints);
            socket.Emit(SyncWorld, CreateJSONObject(ndd));
            Services.WorldUpdateService.StopTime = false;
        }
    }

    void OnReady(SocketIOEvent e)
    {
        Debug.Log("Ready called");
        socket.Emit(EnterWorld, CreateJSONObject(new NetworkIdData()));
        IsConnected = true;
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

            Services.SaveAndLoadService.Save();

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

            //Spawner.Spawn(networkEntity, p == new Point(0, 0) ? Services.DungeonService.DungeonGenerator.Rooms[0].GetValidPoint() : p);
            networkEntity.CleanupComponents();

            var serializedData = networkEntity.Serialize();

            string savePath = Services.SaveAndLoadService.LoadPath;
            if (Directory.Exists($"{savePath}/Blueprints"))
            {
                foreach (var bp in Directory.EnumerateFiles($"{savePath}/Blueprints"))
                    newConnectionNED.Blueprints.Add(File.ReadAllText(bp));
            }
            newConnectionNED.Blueprints = newConnectionNED.Blueprints.Distinct().ToList();

            socket.Emit(SyncCharacter, CreateJSONObject(new NetworkEntityData(serializedData, newConnectionNED.Blueprints)));

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
            SyncWorldWithHost();
            SyncWorldCompleted += SpawnPlayers;
            
        }
    }

    void SpawnPlayers()
    {
        foreach (var player in Services.PlayerManagerService.GetPlayerEntitiesToSpawn())
            SpawnPlayer(player);

        SyncWorldCompleted -= SpawnPlayers;
    }

    public void SpawnPlayer(IEntity e)
    {
        e.AddComponent(new NetworkId(NetworkId));
        string data = e.Serialize();
        List<string> bluePrints = new List<string>();

        string savePath = Services.SaveAndLoadService.LoadPath;
        if (Directory.Exists($"{savePath}/Blueprints"))
        {
            foreach (var bp in Directory.EnumerateFiles($"{savePath}/Blueprints"))
                bluePrints.Add(File.ReadAllText(bp));
        }
        socket.Emit(Spawn, CreateJSONObject(new NetworkEntityData(data, bluePrints)));
    }

    public void SyncWorldWithHost()
    {
        Services.WorldUpdateService.StopTime = true;

        List<string> bluePrints = new List<string>();

        string savePath = Services.SaveAndLoadService.LoadPath;
        if (Directory.Exists($"{savePath}/Blueprints"))
        {
            foreach (var bp in Directory.EnumerateFiles($"{savePath}/Blueprints"))
                bluePrints.Add(File.ReadAllText(bp));
        }

        socket.Emit(SyncWorldRequest, CreateJSONObject(new NetworkDungeonData(null, null, bluePrints)));
    }

    void OnProcessGameEvent(SocketIOEvent e)
    {
        Debug.Log(e.data);
    }

    public void BroadcastGameEvent(GameEventSerializable serializableGameEvent)
    {
        var obj = CreateJSONObject(serializableGameEvent);
        socket.BroadcastMessage(ProcessGameEvent, obj);
    }

    JSONObject CreateJSONObject(object obj)
    {
        return JSONObject.Create(JsonUtility.ToJson(obj).ToString());
    }
}
