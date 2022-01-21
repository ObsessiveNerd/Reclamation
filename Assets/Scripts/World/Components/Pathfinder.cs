using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : GameService
{
    public const float ImpassableWeight = 5000f;
    IPathfindingAlgorithm m_Pathfinder;

    public Pathfinder(IPathfindingAlgorithm algorithm)
    {
        m_Pathfinder = algorithm;
    }

    public void GetPathfindingData(Point p,  out bool blocksMovement, out float weight)
    {
        GameEvent ge = GameEventPool.Get(GameEventId.PathfindingData)
                        .With(EventParameters.BlocksMovement, false)
                        .With(EventParameters.Weight, 0f);

        if(m_Tiles.TryGetValue(p, out Actor tile))
                FireEvent(tile, ge);
        blocksMovement = ge.GetValue<bool>(EventParameters.BlocksMovement);
        weight = ge.GetValue<float>(EventParameters.Weight);
        ge.Release();
    }

    public List<Point> CalculatePath(Point start, Point end)
    {
        m_Pathfinder.Clear();
        return m_Pathfinder.CalculatePath(start, end);
    }
}
