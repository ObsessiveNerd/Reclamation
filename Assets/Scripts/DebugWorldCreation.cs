using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DebugWorldCreation : MonoBehaviour
{
    public GameObject TilePrefab;
    public List<Sprite> TileTexture;
    public Sprite PlayerSprite;
    public Sprite Goblin;
    public Sprite Item;
    public Sprite Wall;
    public Sprite Selection;

    World m_World;

    // Start is called before the first frame update
    void Start()
    {
        IEntity player = EntityFactory.CreateEntity("Dwarf");
        IEntity goblin = EntityFactory.CreateEntity("Goblin");
        
        GameObject world = new GameObject("World");
        Actor worldActor = new Actor("World");
        m_World = new World(worldActor, TilePrefab, TileTexture, Selection);
        worldActor.AddComponent(m_World);
        m_World.RegisterPlayer(player);
        worldActor.CleanupComponents();

        Actor wallActor = new Actor("Wall");
        wallActor.AddComponent(new GraphicContainer(Wall));
        wallActor.AddComponent(new Wall());
        wallActor.AddComponent(new Info("It's a stone wall."));

        wallActor.CleanupComponents();

        worldActor.FireEvent(worldActor, new GameEvent(GameEventId.StartWorld));
        Spawner.Spawn(player, EntityType.Creature, 3, 3);
        Spawner.Spawn(goblin, EntityType.Creature, 10, 12);

        for (int i = 8; i < 16; i++)
            Spawner.Spawn(wallActor, EntityType.Object, i, 10);

        for (int i = 8; i < 16; i++)
            Spawner.Spawn(wallActor, EntityType.Object, 32, i);
    }

    private void Update()
    {
        m_World.ProgressTime();
    }
}
