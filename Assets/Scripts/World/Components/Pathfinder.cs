using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : WorldComponent
{
    public const int ImpassableWeight = 5000;

    IPathfindingAlgorithm m_Pathfinder;
    public override void Init(IEntity self)
    {
        base.Init(self);
        m_Pathfinder = new AStar();

        RegisteredEvents.Add(GameEventId.PathfindingData);
        RegisteredEvents.Add(GameEventId.CalculatePath);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.PathfindingData)
        {
            Point p = gameEvent.GetValue<Point>(EventParameters.TilePosition);
            if(m_Tiles.TryGetValue(p, out Actor tile))
                FireEvent(tile, gameEvent);
        }

        else if(gameEvent.ID == GameEventId.CalculatePath)
        {
            m_Pathfinder.Clear();

            Point startingPoint = gameEvent.GetValue<Point>(EventParameters.StartPos);
            Point targetPoint = gameEvent.GetValue<Point>(EventParameters.EndPos);

            gameEvent.Paramters[EventParameters.Path] = m_Pathfinder.CalculatePath(startingPoint, targetPoint);
        }
    }
}
