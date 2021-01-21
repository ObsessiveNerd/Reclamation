using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldDataQuery : WorldComponent
{

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetEntities);
        RegisteredEvents.Add(GameEventId.GetEntityOnTile);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetEntities)
            gameEvent.Paramters[EventParameters.Value] = GetEntities();

        if (gameEvent.ID == GameEventId.GetEntityOnTile)
        {
            Point currentTilePos = (Point)gameEvent.Paramters[EventParameters.TilePosition];
            FireEvent(m_Tiles[currentTilePos], gameEvent);
        }
    }

    List<IEntity> GetEntities()
    {
        return m_EntityToPointMap.Keys.ToList();
    }
}
