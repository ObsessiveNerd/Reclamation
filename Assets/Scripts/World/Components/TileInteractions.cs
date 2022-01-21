using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInteractions : GameService
{
    public Tile GetTile(Point p)
    {
        if (!m_Tiles.ContainsKey(p))
            return null;
        return m_Tiles[p].GetComponent<Tile>();
    }

    public void TileChanged(Tile t)
    {
        m_ChangedTiles.Add(t);
    }

    public void Pickup(IEntity pickupEntity)
    {
        GameEvent pickup = GameEventPool.Get(GameEventId.Pickup)
                            .With(EventParameters.Entity, pickupEntity.ID);

        Point p = m_EntityToPointMap[pickupEntity];
        FireEvent(m_Tiles[p], pickup);

        pickup.Release();
    }

    public bool IsTileBlocking(Point p)
    {
        if (m_Tiles.ContainsKey(p))
            return m_Tiles[p].GetComponent<Tile>().IsTileBlocking;
        return false;
    }
    public void Drop(IEntity droppingEntity, IEntity entity)
    {
        GameEvent getEntityType = GameEventPool.Get(GameEventId.GetEntityType)
                                    .With(EventParameters.EntityType, EntityType.None);
        FireEvent(entity, getEntityType);

        EntityType entityType = getEntityType.GetValue<EntityType>(EventParameters.EntityType);
        getEntityType.Release();

        if (!m_EntityToPointMap.ContainsKey(droppingEntity))
            return;

        Point p = m_EntityToPointMap[droppingEntity];
        Services.SpawnerService.Spawn(entity, p);
    }

    public string ShowTileInfo(Point pos)
    {
        GameEvent showTileInfo = GameEventPool.Get(GameEventId.ShowTileInfo)
            .With(EventParameters.Info, "");

        string value = FireEvent(m_Tiles[pos], showTileInfo).GetValue<string>(EventParameters.Info);
        showTileInfo.Release();
        return value;
    }

    public void DestroyObject(Point p)
    {
        if (!m_Tiles.ContainsKey(p))
            Debug.Log($"P isn't here. {p}");
        else
            FireEvent(m_Tiles[p], GameEventPool.Get(GameEventId.DestroyObject)).Release();
    }
}
