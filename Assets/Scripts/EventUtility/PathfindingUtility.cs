using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PathfindingUtility
{
    public static MoveDirection GetDirectionTo(IMapNode source, IMapNode target)
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

    public static Point GetRandomValidPoint()
    {
        EventBuilder builder = new EventBuilder(GameEventId.GetRandomValidPoint)
                                .With(EventParameters.Value, null);

        if (World.Instance == null)
            return Point.InvalidPoint;

        return World.Instance.Self.FireEvent(builder.CreateEvent()).GetValue<Point>(EventParameters.Value);
    }

    public static Point GetEntityLocation(IEntity entity)
    {
        EventBuilder getEntityPointBuilder = new EventBuilder(GameEventId.GetEntityLocation)
                                            .With(EventParameters.Entity, entity.ID)
                                            .With(EventParameters.TilePosition, null);
        if (World.Instance == null)
            return Point.InvalidPoint;

        return World.Instance.Self.FireEvent(getEntityPointBuilder.CreateEvent()).GetValue<Point>(EventParameters.TilePosition);
    }

    public static Point GetValidPointWithinRange(IEntity target, int range)
    {
        Point startPos = GetEntityLocation(target);
        return startPos;

        //List<Point> validPoints = new List<Point>();
        //for(int i = startPos.x - range; i < startPos.x + range; i++)
        //{
        //    for(int j = startPos.y - range; j < startPos.y + range; j++)
        //    {
        //        Point p = new Point(i, j);
        //        validPoints.Add(p);
        //    }
        //}

        //if (validPoints.Count == 0 || (validPoints.Count == 1 && validPoints[0] == startPos))
        //    return startPos;

        //if (validPoints.Contains(startPos))
        //    validPoints.Remove(startPos);

        //validPoints = validPoints.OrderBy(x => RecRandom.Instance.GetRandomValue(0, 100)).ToList();
        //foreach(Point vp in validPoints)
        //{
        //    if (IsValidDungeonTile(vp))
        //        return vp;
        //}
        //return startPos;
    }

    public static bool IsValidDungeonTile(Point p)
    {
        EventBuilder isValidPoint = new EventBuilder(GameEventId.IsValidDungeonTile)
                                    .With(EventParameters.TilePosition, p)
                                    .With(EventParameters.Value, false);
        return World.Instance.Self.FireEvent(isValidPoint.CreateEvent()).GetValue<bool>(EventParameters.Value);
    }

    public static bool CanNavigateTo(Point startPos, Point destination)
    {
        EventBuilder calculatePathEventBuilder = new EventBuilder(GameEventId.CalculatePath)
                            .With(EventParameters.StartPos, startPos)
                            .With(EventParameters.EndPos, destination)
                            .With(EventParameters.Path, null);

        var path = World.Instance.Self.FireEvent(calculatePathEventBuilder.CreateEvent()).GetValue<List<IMapNode>>(EventParameters.Path);
        if (path.Count > 0)
            return true;
        return false;
    }

    public static List<IMapNode> GetPath(Point start, Point destination)
    {
        EventBuilder e = new EventBuilder(GameEventId.CalculatePath)
                            .With(EventParameters.StartPos, start)
                            .With(EventParameters.EndPos, destination)
                            .With(EventParameters.Path, null);

        return World.Instance.Self.FireEvent(e.CreateEvent()).GetValue<List<IMapNode>>(EventParameters.Path);
    }
}
