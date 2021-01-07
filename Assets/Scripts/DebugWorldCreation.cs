using System.Collections;
using System.Collections.Generic;
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
        Actor actor = new Actor("Dwarf 1");
        actor.AddComponent(new PlayerInputController(actor));
        actor.AddComponent(new Energy(actor, 1f));
        actor.AddComponent(new GraphicContainter(PlayerSprite));
        actor.AddComponent(new Inventory(actor));
        actor.AddComponent(new Move(actor));
        actor.AddComponent(new Stats(actor, 11, 13, 10, 10, 11, 14));
        actor.AddComponent(new Info(actor, "A stout dwarf."));
        //actor.AddComponent(new Slow(actor));
        //actor.AddComponent(new Drunk(actor));
        actor.CleanupComponents();

        Actor actor2 = new Actor("Dwarf 2");
        actor2.AddComponent(new Energy(actor2, 1f));
        actor2.AddComponent(new GraphicContainter(PlayerSprite));
        actor2.AddComponent(new Inventory(actor2));
        actor2.AddComponent(new Move(actor2));
        //actor2.AddComponent(new Slow(actor2));
        //actor.AddComponent(new Drunk(actor2));
        actor2.CleanupComponents();

        Actor actor3 = new Actor("Goblin");
        actor3.AddComponent(new AIController(actor3));
        actor3.AddComponent(new Energy(actor3, 0.9f));
        actor3.AddComponent(new GraphicContainter(Goblin));
        actor3.AddComponent(new Inventory(actor3));
        actor3.AddComponent(new Move(actor3));
        actor3.AddComponent(new RegisterWithTimeSystem(actor3));
        actor3.AddComponent(new Health(actor3, EntityType.Creature, 10));
        actor3.AddComponent(new Defence(actor3));
        //actor3.AddComponent(new Armor(3));
        //actor.AddComponent(new Slow(actor3));
        //actor3.AddComponent(new Drunk(actor3));
        actor3.CleanupComponents();

        Actor itemActor = new Actor("Pendant");
        itemActor.AddComponent(new Item(itemActor));
        itemActor.AddComponent(new GraphicContainter(Item));
        itemActor.AddComponent(new Info(itemActor, "A pendant.  It looks expensive."));
        itemActor.CleanupComponents();

        GameObject world = new GameObject("World");
        Actor worldActor = new Actor("World");
        m_World = new World(worldActor, TilePrefab, TileTexture, Selection);
        worldActor.AddComponent(m_World);
        m_World.RegisterPlayer(actor);
        m_World.RegisterPlayer(actor2);
        worldActor.CleanupComponents();

        Actor wallActor = new Actor("Wall");
        wallActor.AddComponent(new GraphicContainter(Wall));
        wallActor.AddComponent(new Wall(wallActor));
        wallActor.AddComponent(new Info(wallActor, "It's a stone wall."));

        wallActor.CleanupComponents();

        worldActor.FireEvent(worldActor, new GameEvent(GameEventId.StartWorld));
        Spawner.Spawn(actor, EntityType.Creature, 3, 3);
        Spawner.Spawn(itemActor, EntityType.Item, 10, 12);
        Spawner.Spawn(actor2, EntityType.Creature, 20, 20);
        Spawner.Spawn(actor3, EntityType.Creature, 4, 5);
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
