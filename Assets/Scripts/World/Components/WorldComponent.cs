using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameService //: Component
{
    //protected static Dictionary<IEntity, TimeProgression> m_PlayerToTimeProgressionMap = new Dictionary<IEntity, TimeProgression>();
    protected static TimeProgression m_TimeProgression = new TimeProgression();
    protected static Dictionary<Point, Actor> m_Tiles = new Dictionary<Point, Actor>();

    protected static List<Tile> m_ChangedTiles = new List<Tile>();

    protected static Dictionary<IEntity, Point> m_EntityToPointMap = new Dictionary<IEntity, Point>();
    protected static LinkedList<IEntity> m_Players = new LinkedList<IEntity>();
    protected static LinkedListNode<IEntity> m_ActivePlayer;
    protected static HashSet<Point> m_ValidDungeonPoints = new HashSet<Point>();
    protected static Dictionary<Point, UnityEngine.GameObject> m_GameObjectMap = new Dictionary<Point, UnityEngine.GameObject>();
    protected static Dictionary<int, DungeonGenerationResult> m_DungeonLevelMap = new Dictionary<int, DungeonGenerationResult>();
    protected static int m_CurrentLevel = 1;
    protected static Dictionary<string, IEntity> m_EntityIdToEntityMap = new Dictionary<string, IEntity>();

    public static int CurrentLevel
    {
        get
        {
            return m_CurrentLevel;
        }
    }

    public Point GetPointWhereEntityIs(IEntity e)
    {
        if (m_EntityToPointMap.ContainsKey(e))
            return m_EntityToPointMap[e];
        return new Point(-1, -1);
    }

    protected void Clean()
    {
        m_TimeProgression = new TimeProgression();
        m_Tiles.Clear();
        m_EntityToPointMap.Clear();
        m_Players.Clear();
        m_ActivePlayer = null;
        m_ValidDungeonPoints.Clear();
        foreach (var go in m_GameObjectMap.Values)
            GameObject.Destroy(go);
        m_GameObjectMap.Clear();
        m_DungeonLevelMap.Clear();
        m_CurrentLevel = 1;
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

    protected GameEvent FireEvent(IEntity target, GameEvent gameEvent)
    {
        return target.FireEvent(gameEvent);
    }
}
