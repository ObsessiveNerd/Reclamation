using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ActivatedNetworkObject : NetworkBehaviour
{
    public NetworkVariable<bool> ActivateOnSpawn = new NetworkVariable<bool>(false);
    public override void OnNetworkSpawn()
    {
        if (ActivateOnSpawn.Value)
            Activate();
    }

    public void Activate()
    {
        Services.Spawner.GetEntityFromNetworkId(GetComponent<NetworkObject>().NetworkObjectId, out Entity entity);

        if (entity == null)
            return;

        gameObject.AddComponent<SpriteRenderer>();
        gameObject.AddComponent<BoxCollider2D>();

        foreach (var item in entity.GetComponents())
        {
            var monoType = item.GetType().GetField("MonobehaviorType")?.GetValue(item) as Type;
            if (monoType == null)
                Debug.LogWarning($"{item.GetType()} does not have MonobehaviorType");
            IComponentBehavior monoBehavior = gameObject.AddComponent(monoType) as IComponentBehavior;
            monoBehavior.SetComponent(item);
        }

        //entity.IsActive = true;
        //var entityBehavior = gameObject.GetComponent<EntityBehavior>();
        //if (entityBehavior == null)
        //    entityBehavior = gameObject.AddComponent<EntityBehavior>();
        //entityBehavior.Activate(entity);
    }
}
