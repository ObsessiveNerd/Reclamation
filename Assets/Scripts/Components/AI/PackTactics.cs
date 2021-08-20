using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackTactics : Component
{
    //public bool IsPartyLeader = true;
    //public bool IsInParty
    //{
    //    get
    //    {
    //        return PartyLeaderId != Self.ID;
    //    }
    //}
    public string PartyLeaderId;
    public int PackRange;

    private List<Point> m_VisiblePoints = new List<Point>();
    private Point m_Destination;

    public PackTactics(bool isPartyLeader, bool isInParty, string partyLeaderId, int packRange)
    {
        //IsPartyLeader = isPartyLeader;
        //IsInParty = isInParty;
        PartyLeaderId = partyLeaderId;
        PackRange = packRange;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetActionToTake);
        //RegisteredEvents.Add(GameEventId.GetPackInformation);
        RegisteredEvents.Add(GameEventId.GetCombatRating);
        RegisteredEvents.Add(GameEventId.BreakRank);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetPackInformation)
        {
            gameEvent.Paramters[EventParameters.Entity] = Self.ID;
            //gameEvent.Paramters[EventParameters.IsPartyLeader] = IsPartyLeader;
        }
        else if(gameEvent.ID == GameEventId.BreakRank)
        {
            ClearPartyStatus();
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
        else if(gameEvent.ID == GameEventId.GetActionToTake)
        {
            //if (Self.Name.Contains("Dwarf"))
            //    Debug.LogWarning("Current PL: " + PartyLeaderId);

            if (!string.IsNullOrEmpty(PartyLeaderId))
            {
                if (PartyLeaderId == Self.ID && !Self.HasComponent(typeof(PartyLeader)))
                {
                    PartyLeaderId = null;
                    FindNewPartyLeader();
                }
                else
                {
                    IEntity partyLeader = EntityQuery.GetEntity(PartyLeaderId);
                    if (partyLeader == null)
                    {
                        //IsPartyLeader = true;
                        PartyLeaderId = null;
                        //IsInParty = false;
                        return;
                    }

                    m_Destination = PathfindingUtility.GetValidPointWithinRange(partyLeader, PackRange);

                    AIAction moveWithPack = new AIAction()
                    {
                        Priority = 3,
                        ActionToTake = MoveWithPack
                    };
                    gameEvent.GetValue<PriorityQueue<AIAction>>(EventParameters.AIActionList).Add(moveWithPack);
                }
            }
            else
            {
                FindNewPartyLeader();
            }
        }
    }

    void FindNewPartyLeader()
    {
        if (Self.Name.Contains("Dwarf"))
            Debug.LogWarning("Finding a new party leader");

        EventBuilder getVisiblePoints = new EventBuilder(GameEventId.GetVisibleTiles)
                                            .With(EventParameters.VisibleTiles, new List<Point>());
        m_VisiblePoints = FireEvent(Self, getVisiblePoints.CreateEvent()).GetValue<List<Point>>(EventParameters.VisibleTiles);
        //bool partyLeaderInSight = false;
        foreach (var point in m_VisiblePoints)
        {
            IEntity entityAtTile = WorldUtility.GetEntityAtPosition(point);

            if (entityAtTile == null || entityAtTile == Self)
                continue;

            EventBuilder getPackInformation = new EventBuilder(GameEventId.GetPackInformation)
                                                .With(EventParameters.Entity, null)
                                                .With(EventParameters.IsPartyLeader, false)
                                                .With(EventParameters.TilePosition, point);

            var getInfoEvent = FireEvent(entityAtTile, getPackInformation.CreateEvent());
            var demeanor = Factions.GetDemeanorForTarget(Self, entityAtTile);

            if (getInfoEvent.GetValue<bool>(EventParameters.IsPartyLeader) && demeanor == Demeanor.Friendly)
            {
                //IsPartyLeader = false;
                PartyLeaderId = getInfoEvent.GetValue<string>(EventParameters.Entity);
                //IsInParty = true;
                //partyLeaderInSight = true;
            }

            //if (!partyLeaderInSight)
            //    ClearPartyStatus();
        }
    }

    void ClearPartyStatus()
    {
        if (Self.Name.Contains("Dwarf"))
            Debug.LogWarning("Dwarf is breaking rank");

        //IsPartyLeader = true;
        PartyLeaderId = null;
        //IsInParty = false;
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
        int packRange = 2;

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
                case "PackRange":
                    packRange = int.Parse(keyValue[1]);
                    break;
                case "PartyLeaderId":
                    partyLeaderId = keyValue[1];
                    break;
            }
        }

        Component = new PackTactics(isPartyLeader, isInParty, partyLeaderId, packRange);
    }

    public string CreateSerializableData(IComponent component)
    {
        PackTactics tactics = (PackTactics)component;
        return $"{nameof(PackTactics)}: PackRange={tactics.PackRange}, PartyLeaderId={tactics.PartyLeaderId}";
    }
}
