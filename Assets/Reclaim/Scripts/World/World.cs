//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using SocketIO;
//using UnityEngine;
//using Debug = System.Diagnostics.Debug;

//public class World : MonoBehaviour
//{
//    public GameObject TilePrefab;

//    public DungeonInitMode InitMode;
//    public int MapColumns, MapRows;

//    public enum DungeonInitMode
//    {
//        None = 0,
//        CreateNew,
//        LoadCurrentIfExists,
//        NetworkedGame
//    }

//    private void Start()
//    {
//#if UNITY_EDITOR
//        if (!Services.Ready)
//        {
//            string loadpath = $"{GameSaveSystem.kSaveDataPath}/{Guid.NewGuid().ToString()}";
//            switch (InitMode)
//            {
//                case DungeonInitMode.CreateNew:
//                    StartWorld(loadpath, false);
//                    Services.DungeonService.GenerateDungeon(true, loadpath);
//                    break;

//                case DungeonInitMode.LoadCurrentIfExists:
//                    StartWorld(loadpath, false);

//                    bool value = Directory.Exists(loadpath);
//                    Services.DungeonService.GenerateDungeon(!value, loadpath);
//                    break;
//                case DungeonInitMode.NetworkedGame:
//                    StartWorld(loadpath, true);
//                    break;
//            }
//        }
//#endif
//    }

//    public void StartWorld(string loadPath, bool networkedGame)
//    {
//        if (Services.Ready)
//        {
//            Services.Reset();
//            //GameService.ClearServicesData();
//            Services.Clear();
//        }

//        GameSaveSystem saveSystem = new GameSaveSystem(loadPath);
//        Application.quitting += () => saveSystem.Save();

//        var socket = FindObjectOfType<SocketIOComponent>();
//        Services.Register(new EntityNetworkManager(socket));

//        Services.Complete();
//    }

//    GameEvent m_ProgressTime = new GameEvent(GameEventId.ProgressTime);
//    private void Update()
//    {
//        if(Services.Ready)
//            Services.WorldUpdateService.ProgressTime();
//    }
//}
