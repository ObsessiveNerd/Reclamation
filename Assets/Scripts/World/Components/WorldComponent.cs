using System;
using System.Collections.Generic;

public abstract class WorldComponent : Component
{
    //protected static Dictionary<IEntity, TimeProgression> m_PlayerToTimeProgressionMap = new Dictionary<IEntity, TimeProgression>();
    protected static TimeProgression m_TimeProgression = new TimeProgression();
    protected static Dictionary<Point, Actor> m_Tiles = new Dictionary<Point, Actor>();
    protected static Dictionary<IEntity, Point> m_EntityToPointMap = new Dictionary<IEntity, Point>();
    protected static LinkedList<IEntity> m_Players = new LinkedList<IEntity>();
    protected static LinkedListNode<IEntity> m_ActivePlayer;

    public Point GetPointWhereEntityIs(IEntity e)
    {
        if (m_EntityToPointMap.ContainsKey(e))
            return m_EntityToPointMap[e];
        return new Point(-1, -1);
    }

    protected Point GetTilePointInDirection(Point basePoint, MoveDirection direction)
    {
        if (direction == MoveDirection.None)
            return basePoint;

        int x = basePoint.x;
        int y = basePoint.y;
        string name = Enum.GetName(typeof(MoveDirection), direction);
        if (name.Contains("N"))
            y++;
        if (name.Contains("S"))
            y--;
        if (name.Contains("E"))
            x++;
        if (name.Contains("W"))
            x--;
        return new Point(x, y);
    }
}
