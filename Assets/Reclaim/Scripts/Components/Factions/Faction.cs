using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class FactionData : EntityComponent
{
    public FactionId ID;

    public override void WakeUp()
    {
        RegisteredEvents.Add(GameEventId.GetFaction, GetFaction);
        RegisteredEvents.Add(GameEventId.SetFaction, SetFaction);
        RegisteredEvents.Add(GameEventId.Interact, ResolveIncomingInteraction);
        RegisteredEvents.Add(GameEventId.PrimaryInteraction, ResolveIncomingInteraction);
    }

    void ResolveIncomingInteraction(GameEvent gameEvent)
    {
        GameObject source = gameEvent.GetValue<GameObject>(EventParameter.Source);
        Demeanor demeanor = Factions.GetDemeanorForTarget(source, Entity.GameObject);

        switch (demeanor)
        {
            case Demeanor.Hostile:
                HostileInteractionWithServerRpc(source.GetComponent<NetworkObject>().NetworkObjectId);
                break;
            case Demeanor.Friendly:
            case Demeanor.Neutral:
            case Demeanor.None:
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void HostileInteractionWithServerRpc(ulong sourceId)
    {
        HostileInteractionWithClientRpc(sourceId);
    }

    [ClientRpc]
    void HostileInteractionWithClientRpc(ulong sourceId)
    {
        var interactionSource = NetworkManager.Singleton.SpawnManager.SpawnedObjects[sourceId];

        var triggerAttack = GameEventPool.Get(GameEventId.HostileInteraction)
            .With(EventParameter.Target, Entity.GameObject);
        interactionSource.gameObject.FireEvent(triggerAttack);

        triggerAttack.Release();
    }

    void GetFaction(GameEvent gameEvent)
    {
        gameEvent.Paramters[EventParameter.Value] = this;
    }

    void SetFaction(GameEvent gameEvent)
    {
        ID = gameEvent.GetValue<FactionId>(EventParameter.Faction);
    }
}

public class Faction : EntityComponentBehavior
{
    public FactionData Data = new FactionData();

    public override IComponent GetData()
    {
        return Data;
    }
}
