using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameService //: Component
{
    //protected static Dictionary<GameObject, TimeProgression> m_PlayerToTimeProgressionMap = new Dictionary<GameObject, TimeProgression>();
    protected static TimeProgression m_TimeProgression = new TimeProgression();
    protected static Dictionary<Point, Tile> m_Tiles = new Dictionary<Point, Tile>();
    protected static Dictionary<Point, Actor> m_TileEntity = new Dictionary<Point, Actor>();
    protected static int m_Seed;
    protected static List<Tile> m_ChangedTiles = new List<Tile>();

    protected static Dictionary<string, Point> m_EntityToPointMap = new Dictionary<string, Point>();
    protected static Dictionary<string, Point> m_EntityToPreviousPointMap = new Dictionary<string, Point>();
    protected static LinkedList<GameObject> m_Players = new LinkedList<GameObject>();
    protected static LinkedListNode<GameObject> m_ActivePlayer;
    protected static HashSet<Point> m_ValidDungeonPoints = new HashSet<Point>();
    protected static Dictionary<Point, UnityEngine.GameObject> m_GameObjectMap = new Dictionary<Point, UnityEngine.GameObject>();
    protected static Dictionary<int, DungeonGenerationResult> m_DungeonLevelMap = new Dictionary<int, DungeonGenerationResult>();
    protected static int m_CurrentLevel = 1;
    protected static Dictionary<string, GameObject> m_EntityIdToEntityMap = new Dictionary<string, GameObject>();

    public static void ClearServicesData()
    {
        foreach (var go in m_GameObjectMap.Values)
            GameObject.Destroy(go);
        m_TimeProgression = new TimeProgression();
        m_EntityToPointMap.Clear();
        m_Tiles.Clear();
        m_TileEntity.Clear();
        m_Seed = 0;
        m_ChangedTiles.Clear();
        m_EntityToPreviousPointMap.Clear();
        m_Players.Clear();
        m_ActivePlayer = null;
        m_ValidDungeonPoints.Clear();
        m_GameObjectMap.Clear();
        m_DungeonLevelMap.Clear();
        m_CurrentLevel = 1;
        m_EntityIdToEntityMap.Clear();
    }

    public static int CurrentLevel
    {
        get
        {
            return m_CurrentLevel;
        }
    }

    public Point GetPointWhereEntityIs(GameObject e)
    {
        if (m_EntityToPointMap.ContainsKey(e.ID))
        {
            return m_EntityToPointMap[e.ID];
        }
        if (m_EntityToPreviousPointMap.ContainsKey(e.ID))
        {
            return m_EntityToPreviousPointMap[e.ID];
        }

        //Debug.LogError($"Could not find posiiton for {e.InternalName}");
        return new Point(-1, -1);
    }

    protected void Clean()
    {
        m_TimeProgression = new TimeProgression();
        m_Tiles.Clear();

        var tempMap = new Dictionary<string, Point>(m_EntityToPointMap);
        foreach(var e in tempMap.Keys)
        {
            if (Services.EntityMapService.GetEntity(e) != null && !Services.EntityMapService.GetEntity(e).HasComponent(typeof(Tile)))
                m_EntityToPointMap.Remove(e);
        }

        m_EntityToPreviousPointMap.Clear();
        m_Players.Clear();
        m_ActivePlayer = null;
        m_ValidDungeonPoints.Clear();
        foreach (var go in m_GameObjectMap.Values)
            GameObject.Destroy(go);
        m_GameObjectMap.Clear();
        m_DungeonLevelMap.Clear();
        m_CurrentLevel = 1;
    }

    public  Point GetTilePointInDirection(Point basePoint, MoveDirection direction)
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

    protected GameEvent FireEvent(GameObject target, GameEvent gameEvent)
    {
        return target.FireEvent(gameEvent);
    }
}
