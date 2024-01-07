using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Faction : EntityComponent
{
    public FactionId ID;

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.GetFaction, GetFaction);
        RegisteredEvents.Add(GameEventId.SetFaction, SetFaction);
        RegisteredEvents.Add(GameEventId.Interact, ResolveIncomingInteraction);
    }

    void ResolveIncomingInteraction(GameEvent gameEvent)
    {
        GameObject source = gameEvent.GetValue<GameObject>(EventParameter.Source);
        Demeanor demeanor = Factions.GetDemeanorForTarget(source, gameObject);

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

        var triggerAttack = GameEventPool.Get(GameEventId.PerformMeleeAttack)
            .With(EventParameter.Target, gameObject);
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
