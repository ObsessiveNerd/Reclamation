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
    public static World Instance => m_Instance;

    public IEntity Self => m_World;
    [HideInInspector]
    public int Seed;
    public GameObject TilePrefab;
    IEntity m_World;
#if UNITY_EDITOR
    public bool DebugMode;
    //public bool GenerateDungeonOnStart;
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

        var saveSystem = GameObject.FindObjectOfType<SaveSystem>();
        if(saveSystem != null)
        {
            SaveSystem.Instance.CurrentSaveName = Path.GetFileName(loadPath);
            Application.quitting += () => saveSystem.Save();
        }

        GameEventPool.Initialize();

        m_World = new Actor("World");

        m_World.AddComponent(new WorldSpawner());
        m_World.AddComponent(new DungeonManager());
        m_World.AddComponent(new WorldUpdate());
        m_World.AddComponent(new TileSelection());
        m_World.AddComponent(new TileInteractions());
        m_World.AddComponent(new PlayerManager());
        m_World.AddComponent(new EntityMovement());
        m_World.AddComponent(new WorldUIController());
        m_World.AddComponent(new WorldDataQuery());
        m_World.AddComponent(new WorldFov());
        m_World.AddComponent(new EntityMap());
        m_World.AddComponent(new Pathfinder());
        m_World.AddComponent(new CameraController());
        m_World.AddComponent(new StateManager());
        m_World.AddComponent(new PartyController());

        m_World.CleanupComponents();
    }

    public void GenerateDungeon(bool startNew, string loadPath)
    {
        if (startNew)
        {
            Seed = RecRandom.InitRecRandom(UnityEngine.Random.Range(0, int.MaxValue));
            using (new DiagnosticsTimer("Start World"))
            {
                m_World.FireEvent(m_World, GameEventPool.Get(GameEventId.StartWorld).With(EventParameters.Seed, Seed.ToString())
                                                                                .With(EventParameters.GameObject, TilePrefab)
                                                                                .With(EventParameters.NewGame, true)
                                                                                .With(EventParameters.Level, 1)).Release();
            }
            using (new DiagnosticsTimer("Update world view"))
                m_World.FireEvent(m_World, GameEventPool.Get(GameEventId.UpdateWorldView)).Release();
        }
        else
        {
            FindObjectOfType<SaveSystem>().Load($"{loadPath}/data.save");
        }
    }

    public void InitWorld(int seed, int currentLevel = 1)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Seed = RecRandom.InitRecRandom(seed);
        m_World.FireEvent(m_World, GameEventPool.Get(GameEventId.StartWorld).With(EventParameters.Seed, Seed.ToString())
                                                                            .With(EventParameters.GameObject, TilePrefab)
                                                                            .With(EventParameters.NewGame, false)
                                                                            .With(EventParameters.Level, currentLevel)).Release();
        sw.Stop();
        UnityEngine.Debug.LogWarning($"Start World: {sw.Elapsed.Seconds}");
    }

    GameEvent m_ProgressTime = new GameEvent(GameEventId.ProgressTime);
    private void Update()
    {
        //if (GameEventPool.m_InUse.Count > 0)
        //{
        //    UnityEngine.Debug.Log("Game events weren't released");
        //}

        using(new DiagnosticsTimer("Progress time"))
            m_World?.FireEvent(m_World, m_ProgressTime);
    }
}
