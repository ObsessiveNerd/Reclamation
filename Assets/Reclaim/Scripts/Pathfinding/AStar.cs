using System;
using System.Collections.Generic;

public class AStar : IPathfindingAlgorithm
{
    Dictionary<Point, bool> closedSet;
    Dictionary<Point, bool> openSet;

    //cost of start to this key node
    Dictionary<Point, int> gScore;
    //cost of start to goal, passing through key node
    Dictionary<Point, int> fScore;

    Dictionary<Point, Point> nodeLinks = new Dictionary<Point, Point>();

    public void Clear()
    {
        closedSet.Clear();
        openSet.Clear();
        gScore.Clear();
        fScore.Clear();
        nodeLinks.Clear();
    }

    public AStar(int bufferSize)
    {
        closedSet = new Dictionary<Point, bool>(bufferSize);
        openSet = new Dictionary<Point, bool>(bufferSize);

        //cost of start to this key node
        gScore = new Dictionary<Point, int>(bufferSize);
        //cost of start to goal, passing through key node
        fScore = new Dictionary<Point, int>(bufferSize);
    }

    public List<Point> CalculatePath(Point start, Point goal)
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
        return new List<Point>();
    }

    private int Heuristic(Point start, Point goal)
    {
        var dx = goal.x - start.x;
        var dy = goal.y - start.y;
        int distanceH = Math.Abs(dx) + Math.Abs(dy);

        GameEvent getPathData = GameEventPool.Get(GameEventId.PathfindingData)
                                    //.With(EventParameters.TilePosition, new Point(start.x, start.y))
                                    .With(EventParameters.BlocksMovement, false)
                                    .With(EventParameters.Weight, 1f);

        Tile t = Services.TileInteractionService.GetTile(new Point(start.x, start.y));
        if (t == null)
            return 0;
        t.GetPathFindingData(getPathData);

        float weight = getPathData.GetValue<float>(EventParameters.Weight);
        getPathData.Release();
        return distanceH + (int)weight;
    }

    private int getGScore(Point pt)
    {
        int score = int.MaxValue;
        gScore.TryGetValue(pt, out score);
        return score;
    }

    private int getFScore(Point pt)
    {
        int score = int.MaxValue;
        fScore.TryGetValue(pt, out score);
        return score;
    }

    public IEnumerable<Point> Neighbors(Point center)
    {

        Point pt = new Point(center.x - 1, center.y - 1);
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

    public bool IsValidNeighbor(Point pt)
    {
        Tile t = Services.TileInteractionService.GetTile(pt);
        if (t == null)
            return false;

        return !t.BlocksMovement;
    }

    private List<Point> Reconstruct(Point current)
    {
        List<Point> path = new List<Point>();
        while (nodeLinks.ContainsKey(current))
        {
            path.Add(current);
            current = nodeLinks[current];
        }

        path.Reverse();
        return path;
    }

    private Point NextBest()
    {
        int best = int.MaxValue;
        Point bestPt = Point.InvalidPoint;
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