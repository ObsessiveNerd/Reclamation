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

    public Point GetEntityLocation(GameObject entity)
    {
        if (m_EntityToPointMap.TryGetValue(entity.ID, out Point result))
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

    public GameObject GetClosestEnemy(GameObject source)
    {
        GameObject closestEnemy = null;
        Point sourcePoint = m_EntityToPointMap[source.ID];
        foreach (var tile  in m_Tiles.Values)
        {
            GameObject e = tile.CreatureSlot;
            if (e == null) continue;

            Demeanor demeanor = Factions.GetDemeanorForTarget(source, e);
            if (demeanor == Demeanor.Hostile)
            {
                if (closestEnemy == null)
                    closestEnemy = e;
                else
                {
                    if (Point.Distance(m_EntityToPointMap[e.ID], sourcePoint) < Point.Distance(m_EntityToPointMap[closestEnemy.ID], sourcePoint))
                    {
                        closestEnemy = e;
                    }
                }
            }
        }

        return closestEnemy;
    }

    public GameObject GetClosestAlly(GameObject source)
    {
        GameObject ally = source;
        Point sourcePoint = m_EntityToPointMap[source.ID];
        foreach (var tile  in m_Tiles.Values)
        {
            GameObject e = tile.CreatureSlot;
            if (e == null) continue;

            Demeanor demeanor = Factions.GetDemeanorForTarget(source, e);
            if (demeanor == Demeanor.Friendly)
            {
                if (ally == null)
                    ally = e;
                else
                {
                    if (Point.Distance(m_EntityToPointMap[e.ID], sourcePoint) < Point.Distance(m_EntityToPointMap[ally.ID], sourcePoint))
                    {
                        ally = e;
                    }
                }
            }
        }

        return ally;
    }

    public List<string> GetPlayableCharacters()
    {
        return m_Players.Select(e => e.ID).ToList();
    }

    public GameObject GetGameObject(Point p)
    {
        if (m_GameObjectMap.ContainsKey(p))
            return m_GameObjectMap[p];
        Debug.LogError($"Could not find gameobject for {p}");
        return null;
    }

    public GameObject GetEntityOnTile(Point currentTilePos)
    {
        if (m_Tiles.ContainsKey(currentTilePos))
            return m_Tiles[currentTilePos].GetEntityOnTile();
        return null;
    }

    public List<GameObject> GetEntities()
    {
        return m_EntityToPointMap.Keys.Select(e => m_EntityIdToEntityMap[e]).ToList();
    }
}
