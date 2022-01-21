using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldDataQuery : GameService
{
    public int MapRows;
    public int MapColumns;
    public int Seed { get { return m_Seed; }}
    public WorldDataQuery(int mapRows, int mapColumns)
    {
        MapRows = mapRows;
        MapColumns = mapColumns;
    }

    public string GetActivePlayerId()
    {
        return m_ActivePlayer?.Value.ID;
    }

    public Point GetEntityLocation(IEntity entity)
    {
        if (m_EntityToPointMap.TryGetValue(entity, out Point result))
            return result;
        return Point.InvalidPoint;
    }

    public bool IsValidDungeonTile(Point p)
    {
        return m_ValidDungeonPoints.Contains(p);
    }

    public int GetValueOnTile(Point p)
    {
        return 0;
        //GameEvent getValue = GameEventPool.Get(GameEventId.GetValueOnTile)
        //                        .With(EventParameters.Value, 0);

        //if (m_ValidDungeonPoints.Contains(p))
        //    FireEvent(m_Tiles[p], getValue);

        //int retVal = getValue.GetValue<int>(EventParameters.Value);
        //getValue.Release();
        //return retVal;
    }

    public IEntity GetClosestEnemy(IEntity source)
    {
        IEntity closestEnemy = null;
        float distance = float.MaxValue;
        Point sourcePoint = m_EntityToPointMap[source];
        foreach (var entity in m_EntityToPointMap.Keys)
        {
            if (entity == source)
                continue;

            if (Point.Distance(sourcePoint, m_EntityToPointMap[entity]) < distance &&
                Factions.GetDemeanorForTarget(source, entity) == Demeanor.Hostile)
            {
                closestEnemy = entity;
                distance = Point.Distance(sourcePoint, m_EntityToPointMap[entity]);
            }
        }

        return closestEnemy;
    }

    public List<string> GetPlayableCharacters()
    {
        return m_Players.Select(e => e.ID).ToList();
    }

    public GameObject GetGameObject(Point p)
    {
        if (m_GameObjectMap.ContainsKey(p))
            return m_GameObjectMap[p];
        return null;
    }

    public IEntity GetEntityOnTile(Point currentTilePos)
    {
        if (m_Tiles.ContainsKey(currentTilePos))
            return m_Tiles[currentTilePos].GetEntityOnTile();
        return null;
    }

    public List<IEntity> GetEntities()
    {
        return m_EntityToPointMap.Keys.ToList();
    }
}
