//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class Aggression : EntityComponent
//{
//    Point m_CurrentLocation;
//    Point m_TargetLocation;

//    public void Start()
//    {

//        RegisteredEvents.Add(GameEventId.GetActionToTake, GetActionToTake);
//    }

//    void GetActionToTake(GameEvent gameEvent)
//    {
//        m_CurrentLocation = PathfindingUtility.GetEntityLocation(gameObject);
//        GameEvent getMyAggressionLevel = GameEventPool.Get(GameEventId.GetCombatRating)
//                                                        .With(EventParameter.Value, -1);

//        int myCombatLevel = gameObject.FireEvent(getMyAggressionLevel).GetValue<int>(EventParameter.Value);
//        getMyAggressionLevel.Release();

//        GameEvent getVisiblePoints = GameEventPool.Get(GameEventId.GetVisibleTiles)
//                                            .With(EventParameter.VisibleTiles, new List<Point>());
//        List<Point> visiblePoints = gameObject.FireEvent(getVisiblePoints).GetValue<List<Point>>(EventParameter.VisibleTiles);
//        getVisiblePoints.Release();
//        foreach (var point in visiblePoints)
//        {
//            if (point == m_CurrentLocation)
//                continue;

//            GameObject target = Services.WorldDataQuery.GetEntityOnTile(point);
//            if (target == null)
//                continue;

//            if (Factions.GetDemeanorForTarget(gameObject, target) != Demeanor.Hostile)
//                continue;

//            GameEvent getCombatRatingOfTile = GameEventPool.Get(GameEventId.GetCombatRating)
//                                                        .With(EventParameter.Value, -1);

//            int targetCombatRating = target.FireEvent(getCombatRatingOfTile).GetValue<int>(EventParameter.Value);
//            getCombatRatingOfTile.Release();
//            Debug.Log($"{gameObject.name} combat rating is {myCombatLevel}.  Target {target.name} CR is {targetCombatRating}");
//            if (/*targetCombatRating > -1 && */CombatUtility.ICanTakeThem(myCombatLevel, targetCombatRating))
//            {
//                m_TargetLocation = point;
//                AIAction attackAction = new AIAction()
//                {
//                    Priority = 2,
//                    ActionToTake = MakeAttack
//                };
//                gameEvent.GetValue<PriorityQueue<AIAction>>(EventParameter.AIActionList).Add(attackAction);
//                break;
//            }
//        }
//    }

//    //Todo: will need to check for ranged weapons and perform ranged attack if it wants to
//    MoveDirection MakeAttack()
//    {
//        GameEvent getWeapon = GameEventPool.Get(GameEventId.GetWeapon)
//                                .With(EventParameter.Weapon, new List<string>());
//        var weaponList = gameObject.FireEvent(getWeapon, true).GetValue<List<string>>(EventParameter.Weapon);
//        getWeapon.Release();

//        var spellList = CombatUtility.GetSpells(gameObject);

//        bool useWeapon = weaponList.Any();
//        bool useSpells = spellList.Any();

//        if (useSpells && useWeapon)
//        {
//            if (RecRandom.Instance.GetRandomPercent() < 20)
//                useWeapon = false;
//            else
//                useSpells = false;
//        }

//        if (useWeapon)
//        {
//            foreach (var id in weaponList)
//            {
//                var weapon = EntityQuery.GetEntity(id);
//                List<AttackType> attackTypes = CombatUtility.GetWeaponTypeList(weapon);
//                if (attackTypes.Contains(AttackType.Ranged))
//                {
//                    var target = WorldUtility.GetEntityAtPosition(m_TargetLocation);
//                    CombatUtility.Attack(gameObject, target, EntityQuery.GetEntity(id), AttackType.Ranged);
//                    int howToMove = RecRandom.Instance.GetRandomValue(0, 100);
//                    if (howToMove < 20)
//                        return PathfindingUtility.GetDirectionTo(m_CurrentLocation, m_TargetLocation);
//                    else if (howToMove < 70)
//                        return MoveDirection.None;
//                    else
//                        return PathfindingUtility.GetDirectionAwayFrom(m_CurrentLocation, m_TargetLocation);
//                }
//            }
//        }
//        //else if (useSpells)
//        //{
//        //    var target = WorldUtility.GetEntityAtPosition(m_TargetLocation);
//        //    var spell = spellList[RecRandom.Instance.GetRandomValue(0, spellList.Count)];

//        //    //TODO: we're really getting lazy with all this direct component referencing
//        //    if (spell.HasComponent(typeof(Heal)))
//        //    {
//        //        GameEvent getVisiblePoints = GameEventPool.Get(GameEventId.GetVisibleTiles)
//        //                                    .With(EventParameter.VisibleTiles, new List<Point>());
//        //        List<Point> visiblePoints = gameObject.FireEvent(getVisiblePoints).GetValue<List<Point>>(EventParameter.VisibleTiles);
//        //        getVisiblePoints.Release();

//        //        target = Services.WorldDataQuery.GetClosestAlly(Self);
//        //        var pos = Services.WorldDataQuery.GetPointWhereEntityIs(target);
//        //        if (!visiblePoints.Contains(pos))
//        //            target = Self;
//        //    }

//        //    CombatUtility.Attack(Self, target, spell, AttackType.Spell);

//        //    int howToMove = RecRandom.Instance.GetRandomValue(0, 100);
//        //    if (howToMove < 20)
//        //        return PathfindingUtility.GetDirectionTo(m_CurrentLocation, m_TargetLocation);
//        //    else if (howToMove < 70)
//        //        return MoveDirection.None;
//        //    else
//        //        return PathfindingUtility.GetDirectionAwayFrom(m_CurrentLocation, m_TargetLocation);
//        //}

//        var path = PathfindingUtility.GetPath(m_CurrentLocation, m_TargetLocation);
//        if (path.Count == 0)
//            return MoveDirection.None;

//        return PathfindingUtility.GetDirectionTo(m_CurrentLocation, path[0]);
//    }

//    MoveDirection RunAway()
//    {
//        Point randomPoint = PathfindingUtility.GetRandomValidPoint();
//        var path = PathfindingUtility.GetPath(m_CurrentLocation, randomPoint);
//        gameObject.FireEvent(GameEventPool.Get(GameEventId.BreakRank), true);
//        if (path.Count >= 1)
//            return PathfindingUtility.GetDirectionTo(m_CurrentLocation, path[0]);
//        return PathfindingUtility.GetDirectionTo(m_CurrentLocation, m_CurrentLocation);
//    }
//}
