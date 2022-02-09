using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksNonHostileMovement : EntityComponent
{
    public BlocksNonHostileMovement()
    {
        RegisteredEvents.Add(GameEventId.EntityOvertaking);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if (gameEvent.ID == GameEventId.EntityOvertaking)
        {
            IEntity overtakingEntity = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            Demeanor getDemeanor = Factions.GetDemeanorForTarget(Self, overtakingEntity);
            if (getDemeanor == Demeanor.Friendly || getDemeanor == Demeanor.Neutral)
                if (WorldUtility.IsActivePlayer(overtakingEntity.ID) /*|| !WorldUtility.IsPlayableCharacter(overtakingEntity.ID)*/)
                    Spawner.Swap(Self, overtakingEntity);
            //else

            FireEvent(overtakingEntity, GameEventPool.Get(GameEventId.StopMovement)).Release();
        }
    }
}

public class DTO_BlocksNonHostileMovement : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new BlocksNonHostileMovement();
    }

    public string CreateSerializableData(IComponent component)
    {
        return nameof(BlocksNonHostileMovement);
    }
}