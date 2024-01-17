using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgressionData : ComponentData
{
    public Point CurrentLocation;
    public Point TargetLocation;
}


public class Aggression : EntityComponent
{
    public AgressionData Data;

    Position m_Position;

    public override void WakeUp(IComponentData data = null)
    {
        m_Position = GetComponent<Position>();
        
        if(data == null)
            data = new AgressionData();
        else
            Data = data as AgressionData;

        RegisteredEvents.Add(GameEventId.GetActionToTake, GetActionToTake);
    }

    void GetActionToTake(GameEvent gameEvent)
    {
         Data.CurrentLocation = m_Position.Data.Point;
        GameEvent getVisiblePoints = GameEventPool.Get(GameEventId.GetVisibleTiles)
                                            .With(EventParameter.VisibleTiles, new List<Point>());
        List<Point> visiblePoints = gameObject.FireEvent(getVisiblePoints).GetValue<List<Point>>(EventParameter.VisibleTiles);
        getVisiblePoints.Release();

        foreach (var point in visiblePoints)
        {
            if (point == Data.CurrentLocation)
                continue;

            HashSet<GameObject> targets = Services.Map.GetTile(point).Objects;

            foreach (var target in targets)
            {
                if (Factions.GetDemeanorForTarget(gameObject, target) != Demeanor.Hostile)
                    continue;

                Data.TargetLocation = point;
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
        var path = Services.Pathfinder.GetPath(Data.CurrentLocation, Data.TargetLocation);
        if (path.Count == 0)
            return MoveDirection.None;

        return Services.Pathfinder.GetDirectionTo(Data.CurrentLocation, path[0]);
    }
}
