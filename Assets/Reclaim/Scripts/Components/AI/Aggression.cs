using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Aggression : EntityComponent
{
    Point m_CurrentLocation;
    Point m_TargetLocation;
    Position m_Position;

    public void Start()
    {
        m_Position = GetComponent<Position>();
        RegisteredEvents.Add(GameEventId.GetActionToTake, GetActionToTake);
    }

    void GetActionToTake(GameEvent gameEvent)
    {
        m_CurrentLocation = m_Position.Point;
        GameEvent getVisiblePoints = GameEventPool.Get(GameEventId.GetVisibleTiles)
                                            .With(EventParameter.VisibleTiles, new List<Point>());
        List<Point> visiblePoints = gameObject.FireEvent(getVisiblePoints).GetValue<List<Point>>(EventParameter.VisibleTiles);
        getVisiblePoints.Release();

        foreach (var point in visiblePoints)
        {
            if (point == m_CurrentLocation)
                continue;

            HashSet<GameObject> targets = Services.Map.GetTile(point).Objects;

            foreach (var target in targets)
            {
                if (Factions.GetDemeanorForTarget(gameObject, target) != Demeanor.Hostile)
                    continue;

                m_TargetLocation = point;
                AIAction attackAction = new AIAction()
                {
                    Priority = 2,
                    ActionToTake = MakeAttack
                };
                gameEvent.GetValue<PriorityQueue<AIAction>>(EventParameter.AIActionList).Add(attackAction);
                break;
            }
        }
    }

    MoveDirection MakeAttack()
    {
        var path = Services.Pathfinder.GetPath(m_CurrentLocation, m_TargetLocation);
        if (path.Count == 0)
            return MoveDirection.None;

        return Services.Pathfinder.GetDirectionTo(m_CurrentLocation, path[0]);
    }
}
