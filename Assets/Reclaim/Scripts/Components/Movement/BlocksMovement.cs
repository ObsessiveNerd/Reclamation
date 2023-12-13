using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksNonHostileMovement : EntityComponent
{
    public void Start()
    {
        RegisteredEvents.Add(GameEventId.EntityOvertaking, EntityOvertaking);
    }

    void EntityOvertaking(GameEvent gameEvent)
    {
        GameObject overtakingEntity = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameter.Entity));
        Demeanor getDemeanor = Factions.GetDemeanorForTarget(gameObject, overtakingEntity);
        if (getDemeanor == Demeanor.Friendly || getDemeanor == Demeanor.Neutral)
            if (WorldUtility.IsActivePlayer(overtakingEntity) || overtakingEntity.HasComponent<NetworkController>())
                Spawner.Swap(gameObject, overtakingEntity);
        
        gameObject.FireEvent(overtakingEntity, GameEventPool.Get(GameEventId.StopMovement)).Release();
    }
}