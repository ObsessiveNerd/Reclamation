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
        if(p == Point.InvalidPoint)
        {
            throw new System.Exception("Cannot use invalid point for path finding data");
        }

        GameEvent ge = GameEventPool.Get(GameEventId.PathfindingData)
                        .With(EventParameters.BlocksMovement, false)
                        .With(EventParameters.Weight, 0f);

        if (!m_Tiles.ContainsKey(p))
            throw new System.Exception($"Tiles does not have {p}");

        m_Tiles[p].GetPathFindingData(ge);
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
