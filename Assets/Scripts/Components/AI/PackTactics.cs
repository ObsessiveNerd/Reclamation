using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackTactics : Component
{
    public bool IsPartyLeader = true;
    public bool IsInParty;
    public string PartyLeaderId;
    public int PackRange;

    private Point m_Destination;

    public PackTactics(bool isPartyLeader, bool isInParty, string partyLeaderId, int packRange)
    {
        IsPartyLeader = isPartyLeader;
        IsInParty = isInParty;
        PartyLeaderId = partyLeaderId;
        PackRange = packRange;
    }

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.GetActionToTake);
        RegisteredEvents.Add(GameEventId.GetPackInformation);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetPackInformation)
        {
            gameEvent.Paramters[EventParameters.Entity] = Self.ID;
            gameEvent.Paramters[EventParameters.IsPartyLeader] = IsPartyLeader;
        }

        if(gameEvent.ID == GameEventId.GetActionToTake)
        {
            if (IsInParty && !string.IsNullOrEmpty(PartyLeaderId))
            {
                IEntity partyLeader = EntityQuery.GetEntity(PartyLeaderId);
                if (partyLeader == null)
                {
                    IsPartyLeader = true;
                    PartyLeaderId = null;
                    IsInParty = false;
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
            else
            {
                EventBuilder getVisiblePoints = new EventBuilder(GameEventId.GetVisibleTiles)
                                            .With(EventParameters.VisibleTiles, new List<Point>());
                List<Point> visiblePoints = FireEvent(Self, getVisiblePoints.CreateEvent()).GetValue<List<Point>>(EventParameters.VisibleTiles);
                bool partyLeaderInSight = false;
                foreach (var point in visiblePoints)
                {
                    IEntity entityAtTile = WorldUtility.GetEntityAtPosition(point);

                    if (entityAtTile == null || entityAtTile == Self) continue;

                    EventBuilder getPackInformation = new EventBuilder(GameEventId.GetPackInformation)
                                                        .With(EventParameters.Entity, null)
                                                        .With(EventParameters.IsPartyLeader, false)
                                                        .With(EventParameters.TilePosition, point);

                    var getInfoEvent = FireEvent(entityAtTile, getPackInformation.CreateEvent());
                    var demeanor = Factions.GetDemeanorForTarget(Self, entityAtTile);

                    if (getInfoEvent.GetValue<bool>(EventParameters.IsPartyLeader) && demeanor == Demeanor.Friendly)
                    {
                        IsPartyLeader = false;
                        PartyLeaderId = getInfoEvent.GetValue<string>(EventParameters.Entity);
                        IsInParty = true;
                        partyLeaderInSight = true;
                    }

                    if (!partyLeaderInSight)
                    {
                        IsPartyLeader = true;
                        PartyLeaderId = null;
                        IsInParty = false;
                    }
                }
            }
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
        return $"{nameof(PackTactics)}: IsPartyLeader={tactics.IsPartyLeader}, IsInParty={tactics.IsInParty}, PackRange={tactics.PackRange}, PartyLeaderId={tactics.PartyLeaderId}";
    }
}
