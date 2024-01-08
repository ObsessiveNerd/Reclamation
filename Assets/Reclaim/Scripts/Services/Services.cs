﻿using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Service Locator pattern
/// </summary>
public static class Services
{
    public static bool Ready { get; set; }

    //public static void Reset()
    //{
    //    Ready = false;
    //    InitComplete = Completed;
    //}

    //public static void Complete()
    //{
    //    InitComplete.Invoke(null, null);
    //    InitComplete = null;
    //}

    //public static event EventHandler InitComplete = Completed;
    //private static void Completed(object sender, EventArgs args)
    //{
    //    Ready = true;
    //}

    public static Map Map { get { return GetInstance<Map>(); } }

    //public static WorldSpawner SpawnerService { get { return GetInstance<WorldSpawner>(); } }
    //public static GameSaveSystem SaveAndLoadService { get { return GetInstance<GameSaveSystem>(); } }
    //public static DungeonManager DungeonService { get { return GetInstance<DungeonManager>(); } }
    //public static WorldUpdate WorldUpdateService { get { return GetInstance<WorldUpdate>(); } }
    //public static TileSelection TileSelectionService { get { return GetInstance<TileSelection>(); } }
    //public static TileInteractions TileInteractionService { get { return GetInstance<TileInteractions>(); } }
    //public static PlayerManager PlayerManagerService { get { return GetInstance<PlayerManager>(); } }
    //public static EntityMovement EntityMovementService { get { return GetInstance<EntityMovement>(); } }
    //public static WorldUIController WorldUIService { get { return GetInstance<WorldUIController>(); } }
    //public static WorldDataQuery WorldDataQuery { get { return GetInstance<WorldDataQuery>(); } }
    //public static WorldFov FOVService { get { return GetInstance<WorldFov>(); } }
    //public static EntityMap EntityMapService { get { return GetInstance<EntityMap>(); } }
    //public static Pathfinder PathfinderService { get { return GetInstance<Pathfinder>(); } }
    //public static CameraController CameraService { get { return GetInstance<CameraController>(); } }
    //public static StateManager StateManagerService { get { return GetInstance<StateManager>(); } }
    //public static PartyController PartyService { get { return GetInstance<PartyController>(); } }
    //public static MusicService Music { get { return GetInstance<MusicService>(); } }
    //public static EntityNetworkManager NetworkService { get { return GetInstance<EntityNetworkManager>(); } }

    public static IDictionary<Type, object> _instanceMap = new Dictionary<Type, object>();

    public static void Clear()
    {
        _instanceMap.Clear();
    }

    public static void Register<T>(T instance)
    {
        var type = typeof(T);
        if (_instanceMap.ContainsKey(type))
            throw new InvalidOperationException("An instance of that type already exists: " + type);

        _instanceMap[type] = instance;
    }

    public static T GetInstance<T>()
    {
        return (T)_instanceMap[typeof(T)];
    }

}
