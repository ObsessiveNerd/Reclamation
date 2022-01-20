using System;
using System.Collections.Generic;

public class AStar : IPathfindingAlgorithm
{
    Dictionary<IMapNode, bool> closedSet = new Dictionary<IMapNode, bool>(World.Services.MapColumns * World.Services.MapRows);
    Dictionary<IMapNode, bool> openSet = new Dictionary<IMapNode, bool>(World.Services.MapColumns * World.Services.MapRows);

    //cost of start to this key node
    Dictionary<IMapNode, int> gScore = new Dictionary<IMapNode, int>(World.Services.MapColumns * World.Services.MapRows);
    //cost of start to goal, passing through key node
    Dictionary<IMapNode, int> fScore = new Dictionary<IMapNode, int>(World.Services.MapColumns * World.Services.MapRows);

    Dictionary<IMapNode, IMapNode> nodeLinks = new Dictionary<IMapNode, IMapNode>();

    public void Clear()
    {
        closedSet.Clear();
        openSet.Clear();
        gScore.Clear();
        fScore.Clear();
        nodeLinks.Clear();
    }

    public List<IMapNode> CalculatePath(IMapNode start, IMapNode goal)
    {
        openSet[start] = true;
        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);

        while (openSet.Count > 0)
        {
            var current = NextBest();
            if (current.Equals(goal))
                return Reconstruct(current);

            openSet.Remove(current);
            closedSet[current] = true;

            foreach (var neighbor in Neighbors(current))
            {
                if (closedSet.ContainsKey(neighbor))
                    continue;

                var projectedG = getGScore(current) + 1;

                if (!openSet.ContainsKey(neighbor))
                    openSet[neighbor] = true;
                else if (projectedG >= getGScore(neighbor))
                    continue;

                //record it
                nodeLinks[neighbor] = current;
                gScore[neighbor] = projectedG;
                fScore[neighbor] = projectedG + Heuristic(neighbor, goal);

            }
        }
        return new List<IMapNode>();
    }

    private int Heuristic(IMapNode start, IMapNode goal)
    {
        var dx = goal.x - start.x;
        var dy = goal.y - start.y;
        int distanceH = Math.Abs(dx) + Math.Abs(dy);

        GameEvent getPathData = GameEventPool.Get(GameEventId.PathfindingData)
                                    .With(EventParameters.TilePosition, new Point(start.x, start.y))
                                    .With(EventParameters.BlocksMovement, false)
                                    .With(EventParameters.Weight, 1);

        Tile t = World.Services.Self.GetComponent<TileInteractions>().GetTile(new Point(start.x, start.y));
        t.GetPathFindingData(getPathData);
        int weight = getPathData.GetValue<int>(EventParameters.Weight);
        getPathData.Release();
        return distanceH + weight;
    }

    private int getGScore(IMapNode pt)
    {
        int score = int.MaxValue;
        gScore.TryGetValue(pt, out score);
        return score;
    }

    private int getFScore(IMapNode pt)
    {
        int score = int.MaxValue;
        fScore.TryGetValue(pt, out score);
        return score;
    }

    public IEnumerable<IMapNode> Neighbors(IMapNode center)
    {

        IMapNode pt = new Point(center.x - 1, center.y - 1);
        if (IsValidNeighbor(pt))
            yield return pt;

        pt = new Point(center.x, center.y - 1);
        if (IsValidNeighbor(pt))
            yield return pt;

        pt = new Point(center.x + 1, center.y - 1);
        if (IsValidNeighbor(pt))
            yield return pt;

        //middle row
        pt = new Point(center.x - 1, center.y);
        if (IsValidNeighbor(pt))
            yield return pt;

        pt = new Point(center.x + 1, center.y);
        if (IsValidNeighbor(pt))
            yield return pt;

        //bottom row
        pt = new Point(center.x - 1, center.y + 1);
        if (IsValidNeighbor(pt))
            yield return pt;

        pt = new Point(center.x, center.y + 1);
        if (IsValidNeighbor(pt))
            yield return pt;

        pt = new Point(center.x + 1, center.y + 1);
        if (IsValidNeighbor(pt))
            yield return pt;
    }

    public bool IsValidNeighbor(IMapNode pt)
    {
        //GameEvent getPathData = GameEventPool.Get(GameEventId.PathfindingData)
        //                            .With(EventParameters.TilePosition, pt)
        //                            .With(EventParameters.BlocksMovement, false)
        //                            .With(EventParameters.Weight, 1);

        Tile t = World.Services.Self.GetComponent<TileInteractions>().GetTile(new Point(pt.x, pt.y));
        return t == null ? false : !t.BlocksMovement;
    }

    private List<IMapNode> Reconstruct(IMapNode current)
    {
        List<IMapNode> path = new List<IMapNode>();
        while (nodeLinks.ContainsKey(current))
        {
            path.Add(current);
            current = nodeLinks[current];
        }

        path.Reverse();
        return path;
    }

    private IMapNode NextBest()
    {
        int best = int.MaxValue;
        IMapNode bestPt = null;
        foreach (var node in openSet.Keys)
        {
            var score = getFScore(node);
            if (score < best)
            {
                bestPt = node;
                best = score;
            }
        }
        return bestPt;
    }
}