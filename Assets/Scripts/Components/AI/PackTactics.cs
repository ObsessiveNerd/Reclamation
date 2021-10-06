using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackTactics : Component
{
    public string PartyLeaderId;

    private List<Point> m_VisiblePoints = new List<Point>();
    private Point m_Destination;

    public PackTactics(string partyLeaderId)
    {
        PartyLeaderId = partyLeaderId;
    }

    public override void Start()
    {
        EventBuilder registerWithPartyManager = EventBuilderPool.Get(GameEventId.LookingForGroup)
                                                .With(EventParameters.Entity, Self.ID);
        FireEvent(World.Instance.Self, registerWithPartyManager.CreateEvent());
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetActionToTake);
        RegisteredEvents.Add(GameEventId.GetCombatRating);
        RegisteredEvents.Add(GameEventId.BreakRank);
        RegisteredEvents.Add(GameEventId.SetLeader);
        RegisteredEvents.Add(GameEventId.Died);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.BreakRank)
        {
            ClearPartyStatus();
        }
        else if(gameEvent.ID == GameEventId.Died)
        {
            EventBuilder registerWithPartyManager = EventBuilderPool.Get(GameEventId.RemoveFromParty)
                                                .With(EventParameters.Entity, Self.ID);
            FireEvent(World.Instance.Self, registerWithPartyManager.CreateEvent());
        }
        else if(gameEvent.ID == GameEventId.SetLeader)
        {
            PartyLeaderId = gameEvent.GetValue<string>(EventParameters.Entity);
        }
        else if(gameEvent.ID == GameEventId.GetCombatRating)
        {
            int startValue = (int)gameEvent.Paramters[EventParameters.Value];
            int numberOfAllies = 0;
            foreach(var point in m_VisiblePoints)
            {
                IEntity target = WorldUtility.GetEntityAtPosition(point);
                var demeanor = Factions.GetDemeanorForTarget(Self, target);
                if (demeanor == Demeanor.Friendly)
                    numberOfAllies++;
            }
            gameEvent.Paramters[EventParameters.Value] = startValue + numberOfAllies;
        }
        else if (gameEvent.ID == GameEventId.GetActionToTake)
        {
            if (!string.IsNullOrEmpty(PartyLeaderId) && Self.ID != PartyLeaderId)
            {
                IEntity partyLeader = EntityQuery.GetEntity(PartyLeaderId);
                if (partyLeader == null)
                {
                    PartyLeaderId = null;
                    return;
                }

                m_Destination = PathfindingUtility.GetValidPointWithinRange(partyLeader, 2);

                AIAction moveWithPack = new AIAction()
                {
                    Priority = 3,
                    ActionToTake = MoveWithPack
                };
                gameEvent.GetValue<PriorityQueue<AIAction>>(EventParameters.AIActionList).Add(moveWithPack);
            }
        }
    }

    //void FindNewPartyLeader()
    //{
    //    if (Self.Name.Contains("Dwarf"))
    //        Debug.LogWarning("Finding a new party leader");

    //    EventBuilder getVisiblePoints = EventBuilderPool.Get(GameEventId.GetVisibleTiles)
    //                                        .With(EventParameters.VisibleTiles, new List<Point>());
    //    m_VisiblePoints = FireEvent(Self, getVisiblePoints.CreateEvent()).GetValue<List<Point>>(EventParameters.VisibleTiles);
    //    //bool partyLeaderInSight = false;
    //    foreach (var point in m_VisiblePoints)
    //    {
    //        IEntity entityAtTile = WorldUtility.GetEntityAtPosition(point);

    //        if (entityAtTile == null || entityAtTile == Self)
    //            continue;

    //        EventBuilder getPackInformation = EventBuilderPool.Get(GameEventId.GetPackInformation)
    //                                            .With(EventParameters.Entity, null)
    //                                            .With(EventParameters.IsPartyLeader, false)
    //                                            .With(EventParameters.TilePosition, point);

    //        var getInfoEvent = FireEvent(entityAtTile, getPackInformation.CreateEvent());
    //        var demeanor = Factions.GetDemeanorForTarget(Self, entityAtTile);

    //        if (getInfoEvent.GetValue<bool>(EventParameters.IsPartyLeader) && demeanor == Demeanor.Friendly)
    //        {
    //            //IsPartyLeader = false;
    //            PartyLeaderId = getInfoEvent.GetValue<string>(EventParameters.Entity);
    //            //IsInParty = true;
    //            //partyLeaderInSight = true;
    //        }

    //        //if (!partyLeaderInSight)
    //        //    ClearPartyStatus();
    //    }
    //}

    void ClearPartyStatus()
    {
        PartyLeaderId = null;
    }

    MoveDirection MoveWithPack()
    {
        Point currentLocation = PathfindingUtility.GetEntityLocation(Self);
        var path = PathfindingUtility.GetPath(currentLocation, m_Destination);

        if (path.Count == 0)
            return MoveDirection.None;

        return PathfindingUtility.GetDirectionTo(currentLocation, path[0]);
    }
}

public class DTO_PackTactics : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        bool isPartyLeader = true;
        bool isInParty = false;
        string partyLeaderId = "";
        //int packRange = 2;

        string[] parameters = data.Split(',');
        foreach(var param in parameters)
        {
            string[] keyValue = param.Split('=');
            switch(keyValue[0])
            {
                case "IsPartyLeader":
                    isPartyLeader = bool.Parse(keyValue[1]);
                    break;
                case "IsInParty":
                    isInParty = bool.Parse(keyValue[1]);
                    break;
                //case "PackRange":
                //    packRange = int.Parse(keyValue[1]);
                    //break;
                case "PartyLeaderId":
                    partyLeaderId = keyValue[1];
                    break;
            }
        }

        Component = new PackTactics(partyLeaderId);
    }

    public string CreateSerializableData(IComponent component)
    {
        PackTactics tactics = (PackTactics)component;
        return $"{nameof(PackTactics)}: PartyLeaderId={tactics.PartyLeaderId}";
    }
}
