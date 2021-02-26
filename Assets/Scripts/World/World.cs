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
    public bool StartNew;
    IEntity m_World;
    [HideInInspector]
    public int MapColumns, MapRows;

    // Start is called before the first frame update
    void Start()
    {
        if (m_Instance == null)
            m_Instance = this;
        else
            return;

        Application.quitting += () => GameObject.FindObjectOfType<SaveSystem>().Save();

        int m_Vertical = (int)Camera.main.orthographicSize;
        int m_Horizontal = (int)(m_Vertical * Camera.main.aspect);
        MapColumns = m_Horizontal * 2;
        MapRows = m_Vertical * 2;

        m_World = new Actor("World");

        m_World.AddComponent(new WorldSpawner());
        m_World.AddComponent(new WorldInitialization());
        m_World.AddComponent(new WorldUpdate());
        m_World.AddComponent(new TileSelection());
        m_World.AddComponent(new TileInteractions());
        m_World.AddComponent(new PlayerManager());
        m_World.AddComponent(new EntityMovement());
        m_World.AddComponent(new WorldUIController());
        m_World.AddComponent(new WorldDataQuery());
        m_World.AddComponent(new WorldFov());
        m_World.AddComponent(new EntityMap());
        m_World.CleanupComponents();

        if (StartNew)
        {
            Seed = RecRandom.InitRecRandom(Random.Range(0, int.MaxValue));
            m_World.FireEvent(m_World, new GameEvent(GameEventId.StartWorld, new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.Seed, Seed.ToString()),
                                                                            new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.GameObject, TilePrefab)));

            m_World.FireEvent(m_World, new GameEvent(GameEventId.UpdateWorldView));
        }
        else
        {
            FindObjectOfType<SaveSystem>().Load($"{Directory.EnumerateDirectories(SaveSystem.kSaveDataPath).ToList()[0]}/data.save");
        }
    }

    public void InitWorld(int seed)
    {
        //IEntity player = EntityFactory.CreateEntity("Dwarf");
        //IEntity player2 = EntityFactory.CreateEntity("Dwarf");
        //IEntity goblin = EntityFactory.CreateEntity("Goblin");
        //IEntity helm = EntityFactory.CreateEntity("Helmet");

        //m_World.FireEvent(m_World, new GameEvent(GameEventId.ConvertToPlayableCharacter, new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.Entity, player.ID)));
        //m_World.FireEvent(m_World, new GameEvent(GameEventId.ConvertToPlayableCharacter, new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.Entity, player2.ID)));

        Seed = RecRandom.InitRecRandom(seed);
        m_World.FireEvent(m_World, new GameEvent(GameEventId.StartWorld, new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.Seed, Seed.ToString()),
                                                                            new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.GameObject, TilePrefab)));

        //Spawner.Spawn(player, 0, 0);
        //Spawner.Spawn(player2, 5, 9);
        //Spawner.Spawn(goblin, 10, 12);
    }

    private void Update()
    {
        m_World.FireEvent(m_World, new GameEvent(GameEventId.ProgressTime));
    }
}
