using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class World : MonoBehaviour
{
    private static World m_Instance;
    public GameObject TilePrefab;

#if UNITY_EDITOR
    public bool DebugMode;
#endif

    public enum DungeonInitMode
    {
        None = 0,
        CreateNew,
        LoadCurrentIfExists
    }

    public DungeonInitMode InitMode;

    public int MapColumns, MapRows;

    private void Start()
    {
        if (m_Instance == null)
        {
            //m_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (m_Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
            return;
#if UNITY_EDITOR
        if(DebugMode)
        {
            string loadpath = $"{SaveSystem.kSaveDataPath}/{Guid.NewGuid().ToString()}";
            if (m_Instance == null)
            {
                StartWorld(loadpath);
                switch (InitMode)
                {
                    case DungeonInitMode.CreateNew:
                        GenerateDungeon(true, loadpath);
                        break;

                    case DungeonInitMode.LoadCurrentIfExists:
                        bool value = Directory.Exists(loadpath);
                        GenerateDungeon(!value, loadpath);
                        break;
                }
            }
        }
#endif

    }

    public void StartWorld(string loadPath = "")
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (m_Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
            return;

        SaveSystem saveSystem = new SaveSystem();
        saveSystem.CurrentSaveName = Path.GetFileName(loadPath);
        Application.quitting += () => saveSystem.Save();

        DependencyInjection.Register(saveSystem);
        DependencyInjection.Register(new WorldSpawner());
        DependencyInjection.Register(new DungeonManager(RecRandom.InitRecRandom(DateTime.Now.Second), TilePrefab));
        DependencyInjection.Register(new WorldUpdate());
        DependencyInjection.Register(new TileSelection());
        DependencyInjection.Register(new TileInteractions());
        DependencyInjection.Register(new PlayerManager());
        DependencyInjection.Register(new EntityMovement());
        DependencyInjection.Register(new WorldUIController());
        DependencyInjection.Register(new WorldDataQuery());
        DependencyInjection.Register(new WorldFov());
        DependencyInjection.Register(new EntityMap());
        DependencyInjection.Register(new Pathfinder());
        DependencyInjection.Register(new CameraController());
        DependencyInjection.Register(new StateManager());
        DependencyInjection.Register(new PartyController());

        Services.Complete();
    }

    //public void GenerateDungeon(bool startNew, string loadPath)
    //{
    //    if (startNew)
    //    {
    //        Seed = RecRandom.InitRecRandom(UnityEngine.Random.Range(0, int.MaxValue));
    //        using (new DiagnosticsTimer("Start World"))
    //        {
    //            m_World.FireEvent(m_World, GameEventPool.Get(GameEventId.StartWorld).With(EventParameters.Seed, Seed.ToString())
    //                                                                            .With(EventParameters.GameObject, TilePrefab)
    //                                                                            .With(EventParameters.NewGame, true)
    //                                                                            .With(EventParameters.Level, 1)).Release();
    //        }
    //        using (new DiagnosticsTimer("Update world view"))
    //            m_World.FireEvent(m_World, GameEventPool.Get(GameEventId.UpdateWorldView)).Release();
    //    }
    //    else
    //    {
    //        Services.SaveAndLoadService.Load($"{loadPath}/data.save");
    //    }
    //}

    //public void InitWorld(int seed, int currentLevel = 1)
    //{
    //    Stopwatch sw = new Stopwatch();
    //    sw.Start();
    //    Seed = RecRandom.InitRecRandom(seed);

    //    m_World.FireEvent(m_World, GameEventPool.Get(GameEventId.StartWorld).With(EventParameters.Seed, Seed.ToString())
    //                                                                        .With(EventParameters.GameObject, TilePrefab)
    //                                                                        .With(EventParameters.NewGame, false)
    //                                                                        .With(EventParameters.Level, currentLevel)).Release();
    //    sw.Stop();
    //    UnityEngine.Debug.LogWarning($"Start World: {sw.Elapsed.Seconds}");
    //}

    GameEvent m_ProgressTime = new GameEvent(GameEventId.ProgressTime);
    private void Update()
    {
        //if (GameEventPool.m_InUse.Count > 0)
        //{
        //    UnityEngine.Debug.Log("Game events weren't released");
        //}

        Services.WorldUpdateService.ProgressTime();
    }
}
