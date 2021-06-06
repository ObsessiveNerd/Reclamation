using System;
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
public struct Point : IMapNode
{
    public static readonly Point InvalidPoint = new Point(-1, -1);

    public int x { get; set; }
    public int y { get; set; }

    public Point(int _x, int _y)
    {
        x = _x;
        y = _y;
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
}

public class Tile : Component
{
    public IEntity CreatureSlot;
    public IEntity ObjectSlot;
    public List<IEntity> Items = new List<IEntity>();

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
        RegisteredEvents.Add(GameEventId.IsTileBlocking);
        RegisteredEvents.Add(GameEventId.DestroyObject);
        RegisteredEvents.Add(GameEventId.DestroyAll);
        RegisteredEvents.Add(GameEventId.PathfindingData);
        RegisteredEvents.Add(GameEventId.GetValueOnTile);
        RegisteredEvents.Add(GameEventId.GetInteractableObjects);
        RegisteredEvents.Add(GameEventId.SerializeTile);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateTile)
        {
            IEntity target = m_IsVisible ? GetTarget()[0] : ObjectSlot == null ? Self : ObjectSlot;
            GameEvent getSprite = new GameEvent(GameEventId.GetSprite, new KeyValuePair<string, object>(EventParameters.RenderSprite, null));
            GameEvent getSpriteEvent = FireEvent(target, getSprite);
            FireEvent(Self, new GameEvent(GameEventId.UpdateRenderer, getSpriteEvent.Paramters));
        }

        if(gameEvent.ID == GameEventId.PathfindingData)
        {
            foreach(var target in GetTarget(false))
                FireEvent(target, gameEvent);
        }

        if(gameEvent.ID == GameEventId.DestroyObject)
        {
            Spawner.Despawn(ObjectSlot);
        }

        if (gameEvent.ID == GameEventId.DestroyAll)
        {
            Spawner.Despawn(CreatureSlot);
            Spawner.Despawn(ObjectSlot);
            List<IEntity> items = new List<IEntity>(Items);
            foreach (var item in items)
                Spawner.Despawn(item);
        }

        if (gameEvent.ID == GameEventId.VisibilityUpdated)
        {
            m_IsVisible = (bool)gameEvent.Paramters[EventParameters.Value];
        }

        if (gameEvent.ID == GameEventId.BeforeMoving)
        {
            if (m_HasEntity)
            {
                foreach (IEntity target in GetTarget())
                {
                    GameEvent entityOvertaking = new GameEvent(GameEventId.EntityOvertaking, new KeyValuePair<string, object>(EventParameters.Entity, gameEvent.Paramters[EventParameters.Entity]));
                    FireEvent(target, entityOvertaking);
                    FireEvent(target, gameEvent);
                }
            }
        }

        if (gameEvent.ID == GameEventId.Spawn)
        {
            IEntity entity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            EntityType entityType = (EntityType)gameEvent.Paramters[EventParameters.EntityType];
            switch(entityType)
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
        }

        if(gameEvent.ID == GameEventId.Pickup)
        {
            IEntity entity = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            List<IEntity> itemsPickedup = new List<IEntity>();
            foreach (var item in Items)
            {
                FireEvent(entity, new GameEvent(GameEventId.AddToInventory, new KeyValuePair<string, object>(EventParameters.Entity, item.ID)));
                itemsPickedup.Add(item);
            }

            foreach (var item in itemsPickedup)
                Spawner.Despawn(item);
        }

        if(gameEvent.ID == GameEventId.Despawn)
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
        }

        if (gameEvent.ID == GameEventId.ShowTileInfo)
        {
            GameEvent showInfo = new GameEvent(GameEventId.ShowInfo);
            foreach(var e in GetTarget())
                FireEvent(e, showInfo);
        }

        if (gameEvent.ID == GameEventId.AddComponentToTile)
        {
            //Todo
        }

        if (gameEvent.ID == GameEventId.GetEntityOnTile)
        {
            bool includeSelf = true;
            if(gameEvent.Paramters.ContainsKey(EventParameters.IncludeSelf))
                includeSelf = gameEvent.GetValue<bool>(EventParameters.IncludeSelf);

            var targets = GetTarget(includeSelf);
            if(targets.Count == 0)
                gameEvent.Paramters[EventParameters.Entity] = null;
            else
                gameEvent.Paramters[EventParameters.Entity] = GetTarget(includeSelf)[0].ID;
        }

        if(gameEvent.ID == GameEventId.IsTileBlocking)
        {
            if (GetTarget()[0] != Self)
            {
                foreach (IEntity e in GetTarget())
                    FireEvent(e, gameEvent);
            }
        }

        if(gameEvent.ID == GameEventId.GetValueOnTile)
        {
            if (Items.Count == 0)
                return;

            int totalValue = 0;
            foreach(var item in Items)
            {
                EventBuilder getValue = new EventBuilder(GameEventId.GetValue)
                                        .With(EventParameters.Value, 0);
                int itemValue = FireEvent(item, getValue.CreateEvent()).GetValue<int>(EventParameters.Value);
                totalValue += itemValue;
            }
            gameEvent.Paramters[EventParameters.Value] = totalValue;
        }

        if(gameEvent.ID == GameEventId.SerializeTile)
        {
            LevelData levelData = gameEvent.GetValue<LevelData>(EventParameters.Value);
            foreach (var target in AllEntities)
                if(target != null)
                    levelData.Entities.Add(target.Serialize());
        }
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
