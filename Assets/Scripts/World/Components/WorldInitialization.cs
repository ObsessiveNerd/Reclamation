using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInitialization : WorldComponent
{
    GameObject m_TilePrefab;
    IDungeonGenerator m_DungeonGenerator;
    public string Seed;

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.StartWorld);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.StartWorld)
        {
            Seed = (string)gameEvent.Paramters[EventParameters.Seed];
            m_TilePrefab = (GameObject)gameEvent.Paramters[EventParameters.GameObject];

            m_DungeonGenerator = new BasicDungeonGenerator(m_TilePrefab);

            m_DungeonGenerator.GenerateDungeon(m_Tiles);

            SpawnPlayers();
            SpawnEnemies();

            Factions.Initialize();
            FireEvent(Self, new GameEvent(GameEventId.UpdateWorldView));
        }
    }

    void SpawnPlayers()
    {
        IEntity player = EntityFactory.CreateEntity("Dwarf");
        IEntity player2 = EntityFactory.CreateEntity("Dwarf");

        FireEvent(Self, new GameEvent(GameEventId.ConvertToPlayableCharacter, new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.Entity, player.ID)));
        FireEvent(Self, new GameEvent(GameEventId.ConvertToPlayableCharacter, new System.Collections.Generic.KeyValuePair<string, object>(EventParameters.Entity, player2.ID)));

        Spawner.Spawn(player, m_DungeonGenerator.Rooms[0].GetValidPoint());
        Spawner.Spawn(player2, m_DungeonGenerator.Rooms[0].GetValidPoint());

        player.CleanupComponents();

        FireEvent(player, new GameEvent(GameEventId.InitFOV));
        FireEvent(player2, new GameEvent(GameEventId.InitFOV));

        FireEvent(Self, new GameEvent(GameEventId.ProgressTime));
    }

    void SpawnEnemies()
    {
        Room randomRoom = m_DungeonGenerator.Rooms[RecRandom.Instance.GetRandomValue(1, m_DungeonGenerator.Rooms.Count)];
        IEntity goblin = EntityFactory.CreateEntity("Goblin");
        Spawner.Spawn(goblin, randomRoom.GetValidPoint());
    }
}
