using UnityEngine;

public class World : MonoBehaviour
{
    private static World m_Instance;
    public static World Instance => m_Instance;
    public IEntity Self => m_World;

    public GameObject TilePrefab;
    public bool StartNew;
    IEntity m_World;

    // Start is called before the first frame update
    void Start()
    {
        if (m_Instance == null)
            m_Instance = this;
        else
            return;

        IEntity player = EntityFactory.CreateEntity("Dwarf");
        IEntity goblin = EntityFactory.CreateEntity("Goblin");

        m_World = new Actor("World");

        m_World.AddComponent(new WorldSpawner());
        m_World.AddComponent(new WorldInitialization());
        m_World.AddComponent(new WorldUpdate());
        m_World.AddComponent(new TileSelection());
        m_World.AddComponent(new TileInteractions());
        m_World.AddComponent(new PlayerManager());
        m_World.AddComponent(new EntityMovement());
        m_World.AddComponent(new WorldDataQuery());

        m_World.CleanupComponents();
        m_World.FireEvent(m_World, new GameEvent(GameEventId.StartWorld, new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.Seed, "0"),
                                                                            new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.GameObject, TilePrefab)));

        if (StartNew)
        {
            m_World.FireEvent(m_World, new GameEvent(GameEventId.ConvertToPlayableCharacter, new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.Entity, player)));
            Spawner.Spawn(player, 3, 3);
            Spawner.Spawn(goblin, 10, 12);
        }
        else
            SaveSystem.Load($"{SaveSystem.kSaveDataPath}/{0}/data.save");
    }

    private void Update()
    {
        m_World.FireEvent(m_World, new GameEvent(GameEventId.ProgressTime));
    }
}
