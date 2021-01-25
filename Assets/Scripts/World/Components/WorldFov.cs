using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldFov : WorldComponent
{
    Dictionary<IEntity, List<Point>> m_PlayerToVisibleTiles = new Dictionary<IEntity, List<Point>>();

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.FOVRecalculated);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.FOVRecalculated)
        {
            IEntity source = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            List<Point> newVisibleTiles = (List<Point>)gameEvent.Paramters[EventParameters.VisibleTiles];

            if (!m_PlayerToVisibleTiles.ContainsKey(source))
                m_PlayerToVisibleTiles.Add(source, newVisibleTiles);

            ClearTiles(m_PlayerToVisibleTiles[source]);
            UpdateTiles(newVisibleTiles);
            m_PlayerToVisibleTiles[source] = newVisibleTiles;
        }
    }

    void ClearTiles(List<Point> tiles)
    {
        foreach(Point tile in tiles)
            FireEvent(m_Tiles[tile], new GameEvent(GameEventId.SetVisibility, new KeyValuePair<string, object>(EventParameters.TileInSight, false)));
    }

    void UpdateTiles(List<Point> tiles)
    {
        foreach (Point tile in tiles)
            FireEvent(m_Tiles[tile], new GameEvent(GameEventId.SetVisibility, new KeyValuePair<string, object>(EventParameters.TileInSight, true)));
    }
}
