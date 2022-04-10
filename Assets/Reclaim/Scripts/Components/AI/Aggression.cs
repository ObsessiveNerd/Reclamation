using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aggression : EntityComponent
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
            GameEvent getMyAggressionLevel = GameEventPool.Get(GameEventId.GetCombatRating)
                                                        .With(EventParameters.Value, -1);

            int myCombatLevel = FireEvent(Self, getMyAggressionLevel).GetValue<int>(EventParameters.Value);
            getMyAggressionLevel.Release();

            GameEvent getVisiblePoints = GameEventPool.Get(GameEventId.GetVisibleTiles)
                                            .With(EventParameters.VisibleTiles, new List<Point>());
            List<Point> visiblePoints = FireEvent(Self, getVisiblePoints).GetValue<List<Point>>(EventParameters.VisibleTiles);
            getVisiblePoints.Release();
            foreach(var point in visiblePoints)
            {
                if (point == m_CurrentLocation) continue;

                IEntity target = Services.WorldDataQuery.GetEntityOnTile(point);
                if (target == null) continue;

                if (Factions.GetDemeanorForTarget(Self, target) != Demeanor.Hostile) continue;

                GameEvent getCombatRatingOfTile = GameEventPool.Get(GameEventId.GetCombatRating)
                                                        .With(EventParameters.Value, -1);

                int targetCombatRating = FireEvent(target, getCombatRatingOfTile).GetValue<int>(EventParameters.Value);
                getCombatRatingOfTile.Release();
                Debug.Log($"{Self.Name} combat rating is {myCombatLevel}.  Target {target.Name} CR is {targetCombatRating}");
                if (/*targetCombatRating > -1 && */CombatUtility.ICanTakeThem(myCombatLevel, targetCombatRating))
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
        GameEvent getWeapon = GameEventPool.Get(GameEventId.GetWeapon)
                                .With(EventParameters.Weapon, new List<string>());
        var list = FireEvent(Self, getWeapon, true).GetValue<List<string>>(EventParameters.Weapon);
        getWeapon.Release();
        foreach(var id in list)
        {
            var weapon = EntityQuery.GetEntity(id);
            List<AttackType> attackTypes = CombatUtility.GetWeaponTypeList(weapon);
            if(attackTypes.Contains(AttackType.Ranged))
            {
                var target = WorldUtility.GetEntityAtPosition(m_TargetLocation);
                CombatUtility.Attack(Self, target, EntityQuery.GetEntity(id), AttackType.Ranged);
                int howToMove = RecRandom.Instance.GetRandomValue(0, 100);
                if (howToMove < 20)
                    return PathfindingUtility.GetDirectionTo(m_CurrentLocation, m_TargetLocation);
                else if (howToMove < 70)
                    return MoveDirection.None;
                else
                    return PathfindingUtility.GetDirectionAwayFrom(m_CurrentLocation, m_TargetLocation);
            }
            else if(weapon.HasComponent(typeof(SpellContainer)) && RecRandom.Instance.GetRandomPercent() < 20)
            {
                var target = WorldUtility.GetEntityAtPosition(m_TargetLocation);

                var spells = CombatUtility.GetSpells(EntityQuery.GetEntity(id));
                var spell = spells[RecRandom.Instance.GetRandomValue(0, spells.Count)];

                if (spell.HasComponent(typeof(Heal)))
                    target = Self;

                CombatUtility.CastSpell(Self, target, spell);
                
                int howToMove = RecRandom.Instance.GetRandomValue(0, 100);
                if (howToMove < 20)
                    return PathfindingUtility.GetDirectionTo(m_CurrentLocation, m_TargetLocation);
                else if (howToMove < 70)
                    return MoveDirection.None;
                else
                    return PathfindingUtility.GetDirectionAwayFrom(m_CurrentLocation, m_TargetLocation);
            }
        }

        var path = PathfindingUtility.GetPath(m_CurrentLocation, m_TargetLocation);
        if (path.Count == 0)
            return MoveDirection.None;

        return PathfindingUtility.GetDirectionTo(m_CurrentLocation, path[0]);
    }

    MoveDirection RunAway()
    {
        Point randomPoint = PathfindingUtility.GetRandomValidPoint();
        var path = PathfindingUtility.GetPath(m_CurrentLocation, randomPoint);
        FireEvent(Self, GameEventPool.Get(GameEventId.BreakRank), true);
        if(path.Count >= 1)
            return PathfindingUtility.GetDirectionTo(m_CurrentLocation, path[0]);
        return PathfindingUtility.GetDirectionTo(m_CurrentLocation, m_CurrentLocation);
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
