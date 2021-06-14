using System.IO;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour
{
    private static World m_Instance;
    public static World Instance => m_Instance;
    public IEntity Self => m_World;
    [HideInInspector]
    public int Seed;
    public GameObject TilePrefab;
    IEntity m_World;

    public int MapColumns, MapRows;

    public void StartWorld(bool startNew, string loadPath)
    {
        if (m_Instance == null)
            m_Instance = this;
        else
            return;

        SaveSystem.Instance.CurrentSaveName = Path.GetFileName(loadPath);
        Application.quitting += () => GameObject.FindObjectOfType<SaveSystem>().Save();

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

        m_World.CleanupComponents();

        if (startNew)
        {
            Seed = RecRandom.InitRecRandom(Random.Range(0, int.MaxValue));
            m_World.FireEvent(m_World, new GameEvent(GameEventId.StartWorld, new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.Seed, Seed.ToString()),
                                                                            new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.GameObject, TilePrefab),
                                                                            new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.NewGame, true)));

            m_World.FireEvent(m_World, new GameEvent(GameEventId.UpdateWorldView));
        }
        else
        {
            FindObjectOfType<SaveSystem>().Load($"{loadPath}/data.save");
        }
    }

    public void InitWorld(int seed)
    {
        Seed = RecRandom.InitRecRandom(seed);
        m_World.FireEvent(m_World, new GameEvent(GameEventId.StartWorld, new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.Seed, Seed.ToString()),
                                                                            new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.GameObject, TilePrefab),
                                                                            new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.NewGame, false)));
    }

    private void Update()
    {
        m_World.FireEvent(m_World, new GameEvent(GameEventId.ProgressTime));
    }
}
