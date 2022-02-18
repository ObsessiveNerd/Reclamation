﻿using System;
using UnityEngine;
/// <summary>
/// Service Locator pattern
/// </summary>
public static class Services
{
    public static bool Ready { get; set; }

    public static void Reset()
    {
        Ready = false;
        InitComplete = Completed;
    }

    public static void Complete()
    {
        InitComplete.Invoke(null, null);
        InitComplete = null;
    }

    public static event EventHandler InitComplete = Completed;
    private static void Completed(object sender, EventArgs args)
    {
        Ready = true;
    }

    public static WorldSpawner SpawnerService { get { return DependencyInjection.GetInstance<WorldSpawner>(); } }
    public static GameSaveSystem SaveAndLoadService { get { return DependencyInjection.GetInstance<GameSaveSystem>(); } }
    public static DungeonManager DungeonService { get { return DependencyInjection.GetInstance<DungeonManager>(); } }
    public static WorldUpdate WorldUpdateService { get { return DependencyInjection.GetInstance<WorldUpdate>(); } }
    public static TileSelection TileSelectionService { get { return DependencyInjection.GetInstance<TileSelection>(); } }
    public static TileInteractions TileInteractionService { get { return DependencyInjection.GetInstance<TileInteractions>(); } }
    public static PlayerManager PlayerManagerService { get { return DependencyInjection.GetInstance<PlayerManager>(); } }
    public static EntityMovement EntityMovementService { get { return DependencyInjection.GetInstance<EntityMovement>(); } }
    public static WorldUIController WorldUIService { get { return DependencyInjection.GetInstance<WorldUIController>(); } }
    public static WorldDataQuery WorldDataQuery { get { return DependencyInjection.GetInstance<WorldDataQuery>(); } }
    public static WorldFov FOVService { get { return DependencyInjection.GetInstance<WorldFov>(); } }
    public static EntityMap EntityMapService { get { return DependencyInjection.GetInstance<EntityMap>(); } }
    public static Pathfinder PathfinderService { get { return DependencyInjection.GetInstance<Pathfinder>(); } }
    public static CameraController CameraService { get { return DependencyInjection.GetInstance<CameraController>(); } }
    public static StateManager StateManagerService { get { return DependencyInjection.GetInstance<StateManager>(); } }
    public static PartyController PartyService { get { return DependencyInjection.GetInstance<PartyController>(); } }
    public static MusicService Music { get { return DependencyInjection.GetInstance<MusicService>(); } }
    public static EntityNetworkManager NetworkService { get { return DependencyInjection.GetInstance<EntityNetworkManager>(); } }

}
