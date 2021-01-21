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

            m_DungeonGenerator = new BasicDungeonGenerator(int.Parse(Seed), m_TilePrefab);

            m_DungeonGenerator.GenerateDungeon(m_Tiles);
            Factions.Initialize();
            FireEvent(Self, new GameEvent(GameEventId.UpdateWorldView));
        }
    }
}
