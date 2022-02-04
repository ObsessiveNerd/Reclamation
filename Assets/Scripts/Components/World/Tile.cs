﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PointComparer : IEqualityComparer<Point>
{
    public bool Equals(Point x, Point y)
    {
        return x == y;
    }

    public int GetHashCode(Point obj)
    {
        return obj.GetHashCode();
    }
}

[Serializable]
public struct Point
{
    public static readonly Point InvalidPoint = new Point(-1, -1);

    [SerializeField]
    private int m_x;
    [SerializeField]
    private int m_y;

    public int x { get{ return m_x; } set { m_x = value; } }
    public int y { get{ return m_y; } set { m_y = value; } }

    public Point(int _x, int _y)
    {
        m_x = _x;
        m_y = _y;
    }

    public static Point operator+(Point lhs, Point rhs)
    {
        return new Point(lhs.x + rhs.x, lhs.y + rhs.y);
    }

    public static Point operator -(Point lhs, Point rhs)
    {
        return new Point(lhs.x - rhs.x, lhs.y - rhs.y);
    }

    public override bool Equals(object obj)
    {
        if(obj is Point)
            return ((Point)obj).x == x && ((Point)obj).y == y;
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(Point lhs, Point rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(Point lhs, Point rhs)
    {
        return !lhs.Equals(rhs);
    }

    public static float Distance(Point lhs, Point rhs)
    {
        return Mathf.Sqrt(Mathf.Pow(lhs.x - rhs.x, 2) + Mathf.Pow(lhs.y - rhs.y, 2));
    }

    public override string ToString()
    {
        return $"{x},{y}";
    }
}

public class Tile : EntityComponent
{
    public IEntity CreatureSlot;
    public IEntity ObjectSlot;
    public List<IEntity> Items = new List<IEntity>();
    public bool BlocksMovement
    {
        get
        {
            if (CreatureSlot == null && ObjectSlot == null)
                return false;
            else
            {
                GameEvent b = GameEventPool.Get(GameEventId.PathfindingData)
                                .With(EventParameters.BlocksMovement, false)
                                .With(EventParameters.Weight, 1f);
                foreach (var t in GetTarget(false))
                {
                    var e = FireEvent(t, b);
                    if (e.GetValue<bool>(EventParameters.BlocksMovement))
                    {
                        b.Release();
                        return true;
                    }
                }
                b.Release();
                return false;
            }
        }
    }

    List<IEntity> AllEntities
    {
        get
        {
            List<IEntity> entities = new List<IEntity>(Items);
            entities.Add(CreatureSlot);
            entities.Add(ObjectSlot);
            return entities;
        }
    }
    bool m_HasEntity { get { return CreatureSlot != null || ObjectSlot != null || Items.Count > 0; } }
    bool m_IsVisible = false;
    Point m_GridPoint;

    public Tile(IEntity self, Point gridPoint)
    {
        Init(self);
        m_GridPoint = gridPoint;

        RegisteredEvents.Add(GameEventId.UpdateTile);
        RegisteredEvents.Add(GameEventId.Spawn);
        RegisteredEvents.Add(GameEventId.Despawn);
        RegisteredEvents.Add(GameEventId.ShowTileInfo);
        RegisteredEvents.Add(GameEventId.AddComponentToTile);
        RegisteredEvents.Add(GameEventId.GetEntityOnTile);
        RegisteredEvents.Add(GameEventId.BeforeMoving);
        RegisteredEvents.Add(GameEventId.Pickup);
        RegisteredEvents.Add(GameEventId.VisibilityUpdated);
        RegisteredEvents.Add(GameEventId.BlocksMovement);
        RegisteredEvents.Add(GameEventId.DestroyObject);
        RegisteredEvents.Add(GameEventId.CleanTile);
        RegisteredEvents.Add(GameEventId.PathfindingData);
        RegisteredEvents.Add(GameEventId.GetValueOnTile);
        RegisteredEvents.Add(GameEventId.GetInteractableObjects);
        RegisteredEvents.Add(GameEventId.SerializeTile);
    }

    public void UpdateTile()
    {
        IEntity target = m_IsVisible ? GetTarget()[0] : ObjectSlot == null ? Self : ObjectSlot;
        GameEvent getSprite = GameEventPool.Get(GameEventId.GetSprite).With(EventParameters.RenderSprite, null);
        GameEvent getSpriteEvent = FireEvent(target, getSprite);
        //Self.GetComponent<Renderer>().Image.sprite = getSprite.GetValue<Sprite>(EventParameters.RenderSprite);

        FireEvent(Self, GameEventPool.Get(GameEventId.UpdateRenderer).With(getSpriteEvent.Paramters)).Release();
        getSpriteEvent.Release();
    }

    public void GetPathFindingData(GameEvent gameEvent)
    {
        foreach(var target in GetTarget(false))
                FireEvent(target, gameEvent);
    }


    public override void HandleEvent(GameEvent gameEvent)
    {
        throw new Exception($"Tile does not accept events {gameEvent.ID}");
    }

    public void CleanTile()
    {
        CreatureSlot = null;
        ObjectSlot = null;
        Items.Clear();
        FireEvent(Self, GameEventPool.Get(GameEventId.SetVisibility)
            .With(EventParameters.TileInSight, false)).Release();
        FireEvent(Self, GameEventPool.Get(GameEventId.SetHasBeenVisited)
            .With(EventParameters.HasBeenVisited, false)).Release();
        //Spawner.Despawn(CreatureSlot);
        //Spawner.Despawn(ObjectSlot);
        //List<IEntity> items = new List<IEntity>(Items);
        //foreach (var item in items)
        //    Spawner.Despawn(item);
        Services.TileInteractionService.TileChanged(this);
    }

    public void VisibilityUpdated(GameEvent gameEvent)
    {
        m_IsVisible = (bool)gameEvent.Paramters[EventParameters.Value];
        Services.TileInteractionService.TileChanged(this);
    }

    public void BeforeMoving(GameEvent gameEvent)
    {
        if (m_HasEntity)
        {
            foreach (IEntity target in GetTarget())
            {
                GameEvent entityOvertaking = GameEventPool.Get(GameEventId.EntityOvertaking)
                    .With(EventParameters.Entity, gameEvent.GetValue<string>(EventParameters.Entity)); //.Paramters[EventParameters.Entity]);
                FireEvent(target, entityOvertaking);
                FireEvent(target, gameEvent);

                entityOvertaking.Release();
            }
        }
    }

    public void Spawn(IEntity entity)
    {
        GameEvent getType = GameEventPool.Get(GameEventId.GetEntityType)
                            .With(EventParameters.EntityType, EntityType.None);

        EntityType entityType = entity.FireEvent(getType).GetValue<EntityType>(EventParameters.EntityType);
        getType.Release();

        switch (entityType)
        {
            case EntityType.Creature:
                CreatureSlot = entity;
                break;
            case EntityType.Object:
                ObjectSlot = entity;
                break;
            case EntityType.Item:
                Items.Add(entity);
                break;
        }
        Services.TileInteractionService.TileChanged(this);
    }

    public void Pickup(GameEvent gameEvent)
    {
        IEntity entity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
        if (Items.Count > 0)
        {
            List<IEntity> itemsPickedup = new List<IEntity>();
            foreach (var item in Items)
            {
                var pickupEvent = FireEvent(entity, GameEventPool.Get(GameEventId.AddToInventory)
                    .With(EventParameters.Entity, item.ID));
                itemsPickedup.Add(item);
                pickupEvent.Release();
            }

            foreach (var item in itemsPickedup)
                Spawner.Despawn(item);
        }
        if (ObjectSlot != null)
        {
            FireEvent(ObjectSlot, gameEvent);
        }
        Services.TileInteractionService.TileChanged(this);
    }

    public void Despawn(GameEvent gameEvent)
    {
        IEntity entity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
        EntityType entityType = (EntityType)gameEvent.Paramters[EventParameters.EntityType];
        switch (entityType)
        {
            case EntityType.Creature:
                CreatureSlot = null;
                break;
            case EntityType.Object:
                ObjectSlot = null;
                break;
            case EntityType.Item:
                Items.Remove(entity);
                break;
        }
        Services.TileInteractionService.TileChanged(this);
    }

    public void ShowTileInfo(GameEvent gameEvent)
    {
        GameEvent showInfo = GameEventPool.Get(GameEventId.ShowInfo)
            .With(EventParameters.Info, new StringBuilder());
        foreach (var e in GetTarget())
            FireEvent(e, showInfo);
        gameEvent.Paramters[EventParameters.Info] = showInfo.GetValue<StringBuilder>(EventParameters.Info).ToString();
        showInfo.Release();
    }

    public IEntity GetEntityOnTile(bool includeSelf = true)
    {
        var targets = GetTarget(includeSelf);
        if (targets.Count == 0)
            return null;
        else
            return GetTarget(includeSelf)[0];
    }

    public bool IsTileBlocking
    {
        get
        {
            GameEvent isTileBlocking = GameEventPool.Get(GameEventId.BlocksMovement)
                                        .With(EventParameters.BlocksMovement, false);
            if (GetTarget()[0] != Self)
            {
                foreach (IEntity e in GetTarget())
                    FireEvent(e, isTileBlocking);
            }
            bool returnValue = isTileBlocking.GetValue<bool>(EventParameters.BlocksMovement);
            isTileBlocking.Release();
            return returnValue;
        }
    }

    public bool BlocksVision
    {
        get
        {
            GameEvent isTileBlocking = GameEventPool.Get(GameEventId.BlocksVision)
                                        .With(EventParameters.Value, false);
            if (GetTarget()[0] != Self)
            {
                foreach (IEntity e in GetTarget())
                    FireEvent(e, isTileBlocking);
            }
            bool returnValue = isTileBlocking.GetValue<bool>(EventParameters.Value);
            isTileBlocking.Release();
            return returnValue;
        }
    }

    public void SerializeTile(GameEvent gameEvent)
    {
        DungeonGenerationResult levelData = gameEvent.GetValue<DungeonGenerationResult>(EventParameters.Value);
        foreach (var target in AllEntities)
            if (target != null)
            {
                if (target.HasComponent(typeof(Wall)) || target.HasComponent(typeof(BlocksVisibility)))
                    levelData.Walls.Add(target.Serialize());
                else
                    levelData.Entities.Add(target.Serialize());
            }

        GameEvent getVisibilityData = GameEventPool.Get(GameEventId.GetVisibilityData)
                                            .With(EventParameters.HasBeenVisited, m_IsVisible);

        var getVis = FireEvent(Self, getVisibilityData);
        levelData.TilePoints.Add(m_GridPoint);
        levelData.TileHasBeenVisited.Add(getVis.GetValue<bool>(EventParameters.HasBeenVisited));
        getVis.Release();
    }

    public void DestroyObject()
    {
        Spawner.Despawn(ObjectSlot);
        Services.TileInteractionService.TileChanged(this);
    }

    List<IEntity> GetTarget(bool includeSelf = true)
    {
        //if (m_IsVisible)
        {
            if (CreatureSlot != null)
                return new List<IEntity>() { CreatureSlot };
            else if (ObjectSlot != null)
                return new List<IEntity>() { ObjectSlot };
            else if (Items.Count > 0)
                return Items;
        }

        if(includeSelf)
            return new List<IEntity>() { Self };

        return new List<IEntity>();
    }
}
