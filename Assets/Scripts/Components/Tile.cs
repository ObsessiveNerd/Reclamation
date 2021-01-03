using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point
{
    public int x;
    public int y;

    public Point(int _x, int _y)
    {
        x = _x;
        y = _y;
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
}

public class Tile : Component
{
    public IEntity CreatureSlot;
    public IEntity ObjectSlot;
    public List<IEntity> Items = new List<IEntity>();

    Point m_GridPoint;

    public Tile(IEntity self, Point gridPoint)
    {
        Init(self);
        m_GridPoint = gridPoint;

        RegisteredEvents.Add(GameEventId.UpdateTile);
        RegisteredEvents.Add(GameEventId.Spawn);
        RegisteredEvents.Add(GameEventId.Despawn);
        RegisteredEvents.Add(GameEventId.Interact);
        RegisteredEvents.Add(GameEventId.BeforeMoving);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.UpdateTile)
        {
            IEntity target = Self;
            if (CreatureSlot != null)
                target = CreatureSlot;
            else if (ObjectSlot != null)
                target = ObjectSlot;
            else if (Items.Count > 0)
                target = Items[0];

            GameEvent getSprite = new GameEvent(GameEventId.GetSprite, new KeyValuePair<string, object>(EventParameters.RenderSprite, null));
            FireEvent(target, getSprite);
            Sprite sprite = (Sprite)getSprite.Paramters[EventParameters.RenderSprite];
            FireEvent(Self, new GameEvent(GameEventId.UpdateRenderer, new KeyValuePair<string, object>(EventParameters.RenderSprite, sprite)));
        }

        if(gameEvent.ID == GameEventId.Spawn)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
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

        if(gameEvent.ID == GameEventId.Despawn)
        {
            IEntity entity = (IEntity)gameEvent.Paramters[EventParameters.Entity];
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

        if(gameEvent.ID == GameEventId.Interact)
        {
            //Todo need to be able to interact with neighbors and with the create/object slots
            foreach (IEntity item in Items)
                FireEvent(item, new GameEvent(GameEventId.Pickup, new KeyValuePair<string, object>(EventParameters.Entity, gameEvent.Paramters[EventParameters.Entity])));
            Items.Clear();
        }

        if(gameEvent.ID == GameEventId.BeforeMoving)
        {
            if (CreatureSlot != null)
                FireEvent(CreatureSlot, gameEvent);
            else if (ObjectSlot != null)
                FireEvent(ObjectSlot, gameEvent);
            else if (Items.Count > 0)
            {
                foreach (IEntity item in Items)
                    FireEvent(item, gameEvent);
            }
        }
    }
}
