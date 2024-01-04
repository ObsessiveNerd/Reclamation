using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInteractions : GameService
{
    //public Tile GetTile(Point p)
    //{
    //    if (!m_Tiles.ContainsKey(p))
    //        return null;
    //    return m_Tiles[p];
    //}

    //public void EntityChanged(GameObject e)
    //{
    //    if(m_EntityToPointMap.TryGetValue(e.ID, out Point pos))
    //        TileChanged(m_Tiles[pos]);
    //}

    //public void TileChanged(Tile t)
    //{
    //    m_ChangedTiles.Add(t);
    //}

    //public void Pickup(GameObject pickupEntity)
    //{
    //    GameEvent pickup = GameEventPool.Get(GameEventId.Pickup)
    //                        .With(EventParameter.Entity, pickupEntity.ID);

    //    Point p = m_EntityToPointMap[pickupEntity.ID];
    //    m_Tiles[p].Pickup(pickup);

    //    pickup.Release();
    //}

    //public bool TileBlocksMovement(Point p)
    //{
    //    if (m_Tiles.ContainsKey(p))
    //        return m_Tiles[p].BlocksMovement;
    //    return false;
    //}

    //public bool TileBlocksVision(Point p)
    //{
    //     if (m_Tiles.ContainsKey(p))
    //        return m_Tiles[p].BlocksVision;
    //    return false;
    //}

    //public void Drop(GameObject droppingEntity, GameObject entity)
    //{
    //    if (m_Players.Contains(droppingEntity))
    //        droppingEntity = m_ActivePlayer.Value;

    //    GameEvent getEntityType = GameEventPool.Get(GameEventId.GetEntityType)
    //                                .With(EventParameter.EntityType, EntityType.None);
    //    FireEvent(entity, getEntityType);

    //    EntityType entityType = getEntityType.GetValue<EntityType>(EventParameter.EntityType);
    //    getEntityType.Release();

    //    if (!m_EntityToPointMap.ContainsKey(droppingEntity.ID))
    //        return;

    //    Point p = m_EntityToPointMap[droppingEntity.ID];
    //    Services.SpawnerService.Spawn(entity, p);
    //}

    //public string ShowTileInfo(Point pos)
    //{
    //    GameEvent showTileInfo = GameEventPool.Get(GameEventId.ShowTileInfo)
    //        .With(EventParameter.Info, "");

    //    m_Tiles[pos].ShowTileInfo(showTileInfo);
    //    string value = showTileInfo.GetValue<string>(EventParameter.Info);
    //    showTileInfo.Release();
    //    return value;
    //}

    //public void DestroyObject(Point p)
    //{
    //    if (!m_Tiles.ContainsKey(p))
    //        Debug.Log($"P isn't here. {p}");
    //    else
    //        m_Tiles[p].DestroyObject();
    //}
}
