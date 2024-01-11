using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour //: GameService
{
    public const float ImpassableWeight = 5000f;
    public int BufferSize;
    IPathfindingAlgorithm m_Pathfinder;

    void Start()
    {
        Services.Register(this);
        m_Pathfinder = new AStar(BufferSize);
    }

    public List<Point> GetPath(Point start, Point end)
    {
        return m_Pathfinder.CalculatePath(start, end);
    }

    public MoveDirection GetDirectionTo(Point source, Point target)
    {
        if (source == target)
            return MoveDirection.None;

        int rawX = target.x - source.x;
        int rawY = target.y - source.y;

        float xDir = rawX != 0 ? Mathf.Sign(target.x - source.x) : 0f;
        float yDir = rawY != 0 ? Mathf.Sign(target.y - source.y) : 0f;

        string direction = string.Empty;
        if (yDir < 0)
            direction += "S";
        else if (yDir > 0)
            direction += "N";

        if (xDir < 0)
            direction += "W";
        else if (xDir > 0)
            direction += "E";

        if (Enum.TryParse(direction, out MoveDirection result))
            return result;

        return MoveDirection.None;
    }

    public List<Point> CalculatePath(Point start, Point end)
    {
        return m_Pathfinder.CalculatePath(start, end);
    }
}
