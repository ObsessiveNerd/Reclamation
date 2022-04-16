using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PathfindingUtility
{
    public static MoveDirection GetDirectionAwayFrom(Point source, Point target)
    {
        if (source == target)
            return MoveDirection.None;

        int rawX = target.x - source.x;
        int rawY = target.y - source.y;

        float xDir = rawX != 0 ? Mathf.Sign(target.x - source.x) : 0f;
        float yDir = rawY != 0 ? Mathf.Sign(target.y - source.y) : 0f;

        string direction = string.Empty;
        if (yDir < 0)
            direction += "N";
        else if (yDir > 0)
            direction += "S";

        if (xDir < 0)
            direction += "E";
        else if (xDir > 0)
            direction += "W";

        if (Enum.TryParse(direction, out MoveDirection result))
            return result;

        return MoveDirection.None;
    }

    public static MoveDirection GetDirectionTo(IEntity source, IEntity target)
    {
        Point sourceP = Services.EntityMapService.GetPointWhereEntityIs(source);
        Point targetP = Services.EntityMapService.GetPointWhereEntityIs(target);

        return GetDirectionTo(sourceP, targetP);
    }

    public static MoveDirection GetDirectionTo(Point source, Point target)
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
        return Services.DungeonService.GetRandomValidPoint();
    }

    public static Point GetEntityLocation(IEntity entity)
    {
        return Services.WorldDataQuery.GetEntityLocation(entity);
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

    //public static bool IsValidDungeonTile(Point p)
    //{
    //    GameEvent isValidPoint = GameEventPool.Get(GameEventId.IsValidDungeonTile)
    //                                .With(EventParameters.TilePosition, p)
    //                                .With(EventParameters.Value, false);
    //    return World.Instance.Self.FireEvent(isValidPoint.CreateEvent()).GetValue<bool>(EventParameters.Value);
    //}

    //public static bool CanNavigateTo(Point startPos, Point destination)
    //{
    //    GameEvent calculatePathEventBuilder = GameEventPool.Get(GameEventId.CalculatePath)
    //                        .With(EventParameters.StartPos, startPos)
    //                        .With(EventParameters.EndPos, destination)
    //                        .With(EventParameters.Path, null);

    //    var path = World.Instance.Self.FireEvent(calculatePathEventBuilder.CreateEvent()).GetValue<List<Point>>(EventParameters.Path);
    //    if (path.Count > 0)
    //        return true;
    //    return false;
    //}

    public static List<Point> GetPath(Point start, Point destination)
    {
        return Services.PathfinderService.CalculatePath(start, destination);
    }
}
