using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldDataQuery : WorldComponent
{

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetEntities);
        RegisteredEvents.Add(GameEventId.GetEntityOnTile);
        RegisteredEvents.Add(GameEventId.GetEntityLocation);
        RegisteredEvents.Add(GameEventId.IsValidDungeonTile);
        RegisteredEvents.Add(GameEventId.GetValueOnTile);
        RegisteredEvents.Add(GameEventId.GetClosestEnemy);
        RegisteredEvents.Add(GameEventId.GetPlayableCharacters);
        RegisteredEvents.Add(GameEventId.GetActivePlayerId);
        RegisteredEvents.Add(GameEventId.GameObject);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetEntities)
            gameEvent.Paramters[EventParameters.Value] = GetEntities();

        if (gameEvent.ID == GameEventId.GetEntityOnTile)
        {
            Point currentTilePos = (Point)gameEvent.Paramters[EventParameters.TilePosition];
            if(m_Tiles.ContainsKey(currentTilePos))
                FireEvent(m_Tiles[currentTilePos], gameEvent);
        }

        if (gameEvent.ID == GameEventId.GetActivePlayerId)
            gameEvent.Paramters[EventParameters.Value] = m_ActivePlayer?.Value.ID;

        if(gameEvent.ID == GameEventId.GetEntityLocation)
        {
            EventBuilder eBuilder = new EventBuilder(GameEventId.GetEntity)
                                    .With(EventParameters.Entity, null)
                                    .With(EventParameters.Value, gameEvent.Paramters[EventParameters.Entity]);

            if (m_EntityToPointMap.TryGetValue(FireEvent(Self, eBuilder.CreateEvent()).GetValue<IEntity>(EventParameters.Entity), out Point result))
                gameEvent.Paramters[EventParameters.TilePosition] = result;
        }

        else if(gameEvent.ID == GameEventId.IsValidDungeonTile)
        {
            Point p = gameEvent.GetValue<Point>(EventParameters.TilePosition);
            if (m_ValidDungeonPoints.Contains(p))
                gameEvent.Paramters[EventParameters.Value] = true;
            else
                gameEvent.Paramters[EventParameters.Value] = false;
        }

        else if(gameEvent.ID == GameEventId.GetValueOnTile)
        {
            Point p = gameEvent.GetValue<Point>(EventParameters.TilePosition);
            if (m_ValidDungeonPoints.Contains(p))
                FireEvent(m_Tiles[p], gameEvent);
        }
        else if(gameEvent.ID == GameEventId.GetClosestEnemy)
        {
            IEntity closestEnemy = null;
            float distance = float.MaxValue;
            IEntity source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            Point sourcePoint = m_EntityToPointMap[source];
            foreach (var entity in m_EntityToPointMap.Keys)
            {
                if (entity == source) continue;

                if (Point.Distance(sourcePoint, m_EntityToPointMap[entity]) < distance &&
                    Factions.GetDemeanorForTarget(source, entity) == Demeanor.Hostile)
                {
                    closestEnemy = entity;
                    distance = Point.Distance(sourcePoint, m_EntityToPointMap[entity]);
                }
            }

            gameEvent.Paramters[EventParameters.Value] = closestEnemy?.ID;
        }
        else if(gameEvent.ID == GameEventId.GetPlayableCharacters)
        {
            gameEvent.GetValue<List<string>>(EventParameters.Value).AddRange(m_Players.Select(e => e.ID));
        }

        else if(gameEvent.ID == GameEventId.GameObject)
        {
            Point p = gameEvent.GetValue<Point>(EventParameters.Point);
            if(m_GameObjectMap.ContainsKey(p))
            {
                gameEvent.Paramters[EventParameters.Value] = m_GameObjectMap[p];
            }
        }
    }

    List<IEntity> GetEntities()
    {
        return m_EntityToPointMap.Keys.ToList();
    }
}
