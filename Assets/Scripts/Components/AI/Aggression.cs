using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aggression : Component
{
    Point m_CurrentLocation;
    Point m_TargetLocation;

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetActionToTake);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetActionToTake)
        {
            m_CurrentLocation = PathfindingUtility.GetEntityLocation(Self);
            EventBuilder getMyAggressionLevel = EventBuilderPool.Get(GameEventId.GetCombatRating)
                                                        .With(EventParameters.Value, -1);

            int myCombatLevel = FireEvent(Self, getMyAggressionLevel.CreateEvent()).GetValue<int>(EventParameters.Value);

            EventBuilder getVisiblePoints = EventBuilderPool.Get(GameEventId.GetVisibleTiles)
                                            .With(EventParameters.VisibleTiles, new List<Point>());
            List<Point> visiblePoints = FireEvent(Self, getVisiblePoints.CreateEvent()).GetValue<List<Point>>(EventParameters.VisibleTiles);
            foreach(var point in visiblePoints)
            {
                if (point == m_CurrentLocation) continue;

                EventBuilder getEntity = EventBuilderPool.Get(GameEventId.GetEntityOnTile)
                                                        .With(EventParameters.TilePosition, point)
                                                        .With(EventParameters.Entity, "");

                IEntity target = EntityQuery.GetEntity(FireEvent(World.Instance.Self, getEntity.CreateEvent()).GetValue<string>(EventParameters.Entity));
                if (target == null) continue;

                if (Factions.GetDemeanorForTarget(Self, target) != Demeanor.Hostile) continue;

                EventBuilder getCombatRatingOfTile = EventBuilderPool.Get(GameEventId.GetCombatRating)
                                                        .With(EventParameters.Value, -1);

                int targetCombatRating = FireEvent(target, getCombatRatingOfTile.CreateEvent()).GetValue<int>(EventParameters.Value);
                //Debug.Log($"{Self.Name} combat rating is {myCombatLevel}.  Target {target.Name} CR is {targetCombatRating}");
                if(targetCombatRating > -1 && CombatUtility.ICanTakeThem(myCombatLevel, targetCombatRating))
                {
                    m_TargetLocation = point;
                    AIAction attackAction = new AIAction()
                    {
                        Priority = 2,
                        ActionToTake = MakeAttack
                    };
                    gameEvent.GetValue<PriorityQueue<AIAction>>(EventParameters.AIActionList).Add(attackAction);
                    break;
                }
            }
        }
    }

    //Todo: will need to check for ranged weapons and perform ranged attack if it wants to
    MoveDirection MakeAttack()
    {
        EventBuilder getWeapon = EventBuilderPool.Get(GameEventId.GetWeapon)
                                .With(EventParameters.Weapon, new List<string>());
        var list = FireEvent(Self, getWeapon.CreateEvent()).GetValue<List<string>>(EventParameters.Weapon);
        foreach(var id in list)
        {
            var weapon = EntityQuery.GetEntity(id);
            TypeWeapon weaponType = CombatUtility.GetWeaponType(weapon);
            if(weaponType == TypeWeapon.Ranged)
            {
                var target = WorldUtility.GetEntityAtPosition(m_TargetLocation);
                CombatUtility.Attack(Self, target, EntityQuery.GetEntity(id));
                return MoveDirection.None;
            }
            else if(weaponType == TypeWeapon.Wand || weaponType == TypeWeapon.MagicStaff)
            {
                var target = WorldUtility.GetEntityAtPosition(m_TargetLocation);
                CombatUtility.CastSpell(Self, target, EntityQuery.GetEntity(id));
                return MoveDirection.None;
            }
        }
        var path = PathfindingUtility.GetPath(m_CurrentLocation, m_TargetLocation);
        if (path.Count == 0)
            return MoveDirection.None;

        return PathfindingUtility.GetDirectionTo(m_CurrentLocation, path[0]);
    }
}

public class DTO_Aggression : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        //string[] parameters = data.Split('=');
        //int baseAggression = int.Parse(parameters[1]);
        Component = new Aggression();
    }

    public string CreateSerializableData(IComponent component)
    {
        Aggression agg = (Aggression)component;
        return $"{nameof(Aggression)}"; //:BaseAggression={agg.BaseAggression}";
    }
}
