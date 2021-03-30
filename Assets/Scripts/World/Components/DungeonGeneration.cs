using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneration : WorldComponent
{
    GameObject m_TilePrefab;
    IDungeonGenerator m_DungeonGenerator;
    public string Seed;
    int m_Vertical, m_Horizontal;

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.StartWorld);
        RegisteredEvents.Add(GameEventId.GetRandomValidPoint);
        RegisteredEvents.Add(GameEventId.AddValidPoints);

        m_Vertical = (int)Camera.main.orthographicSize;
        m_Horizontal = (int)(m_Vertical * Camera.main.aspect);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.StartWorld)
        {
            Seed = (string)gameEvent.Paramters[EventParameters.Seed];
            m_TilePrefab = (GameObject)gameEvent.Paramters[EventParameters.GameObject];

            CreateTiles(m_Tiles);

            m_DungeonGenerator = new BasicDungeonGenerator();
            m_DungeonGenerator.GenerateDungeon(World.Instance.MapRows, World.Instance.MapColumns);

            SpawnPlayers();
            SpawnEnemies();
            SpawnItems();

            ///
            //Point p1 = m_DungeonGenerator.Rooms[0].GetValidPoint();
            //Point p2 = m_DungeonGenerator.Rooms[m_DungeonGenerator.Rooms.Count - 1].GetValidPoint();

            //EventBuilder e = new EventBuilder(GameEventId.CalculatePath)
            //                .With(EventParameters.StartPos, p1)
            //                .With(EventParameters.EndPos, p2)
            //                .With(EventParameters.Path, null);

            //var path = FireEvent(Self, e.CreateEvent()).GetValue<List<IMapNode>>(EventParameters.Path);

            //Debug.Log($"StartNode: {p1.x}, {p1.y}");

            //foreach (var node in path)
            //{
            //    Spawner.Spawn(EntityFactory.CreateEntity("Helmet"), node.x, node.y);
            //    Debug.Log($"{node.x}, {node.y}");
            //}

            //Debug.Log($"EndNode: {p2.x}, {p2.y}");
            //

            Factions.Initialize();
            FireEvent(Self, new GameEvent(GameEventId.UpdateWorldView));
        }
        else if(gameEvent.ID == GameEventId.GetRandomValidPoint)
        {
            int roomIndex = RecRandom.Instance.GetRandomValue(0, m_DungeonGenerator.Rooms.Count - 1);
            gameEvent.Paramters[EventParameters.Value] = m_DungeonGenerator.Rooms[roomIndex].GetValidPoint();
        }
        else if(gameEvent.ID == GameEventId.AddValidPoints)
        {
            List<Point> validPoints = gameEvent.GetValue<List<Point>>(EventParameters.Value);
            foreach(Point p in validPoints)
                m_ValidDungeonPoints.Add(p);
        }
    }

    void SpawnPlayers()
    {
        for (int i = 0; i < 1; i++)
        {
            IEntity player = EntityFactory.CreateEntity("Dwarf");
            FireEvent(Self, new GameEvent(GameEventId.ConvertToPlayableCharacter, new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.Entity, player.ID)));
            Spawner.Spawn(player, m_DungeonGenerator.Rooms[0].GetValidPoint());

            if (i == 0)
                player.CleanupComponents();

            FireEvent(player, new GameEvent(GameEventId.InitFOV));
        }

        FireEvent(Self, new GameEvent(GameEventId.ProgressTime));
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < 1; i++)
        {
            Room randomRoom = m_DungeonGenerator.Rooms[RecRandom.Instance.GetRandomValue(1, m_DungeonGenerator.Rooms.Count)];
            IEntity goblin = EntityFactory.CreateEntity("Goblin");
            Spawner.Spawn(goblin, randomRoom.GetValidPoint());
        }
    }

    void SpawnItems()
    {
        Room randomRoom = m_DungeonGenerator.Rooms[RecRandom.Instance.GetRandomValue(1, m_DungeonGenerator.Rooms.Count)];
        IEntity helmet = EntityFactory.CreateEntity("Helmet");
        Spawner.Spawn(helmet, randomRoom.GetValidPoint());
    }

    void CreateTiles(Dictionary<Point, Actor> pointToTileMap)
    {
        for (int i = 0; i < World.Instance.MapColumns; i++)
        {
            for (int j = 0; j < World.Instance.MapRows; j++)
            {
                CreateTile(i, j, m_Horizontal, m_Vertical, pointToTileMap);
            }
        }
    }

    protected void CreateTile(int x, int y, float screenHorizontal, float screenVertical, Dictionary<Point, Actor> pointToTileMap)
    {
        GameObject tile = UnityEngine.GameObject.Instantiate(m_TilePrefab);
        tile.transform.position = new Vector2(x - (screenHorizontal - 0.5f), y - (screenVertical - 0.5f));
        tile.transform.parent = UnityEngine.GameObject.Find("World").transform;

        Actor actor = new Actor("Tile");
        actor.AddComponent(new Tile(actor, new Point(x, y)));
        actor.AddComponent(new TileVisible(false));
        actor.AddComponent(new GraphicContainer("Textures/td_world_floor_cobble_b-120"));
        actor.AddComponent(new Renderer(tile.GetComponent<SpriteRenderer>()));
        actor.CleanupComponents();

        pointToTileMap.Add(new Point(x, y), actor);
    }
}
